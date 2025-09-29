using System.Security.Claims;
using System.Text;
using ClickHouse.Driver.ADO;
using Common.Messages.Agent.Sync;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Server.Api.Endpoints;
using Server.Application.Abstractions.Notifiers;
using Server.Application.Abstractions.Providers;
using Server.Application.Abstractions.Repositories;
using Server.Application.Services;
using Server.Infrastructure.Communication;
using Server.Infrastructure.Configs;
using Server.Infrastructure.Hubs;
using Server.Infrastructure.Notifiers;
using Server.Infrastructure.Repositories.Relational;
using Server.Infrastructure.Repositories.TimeSeries;
using Server.Infrastructure.Utils;

var builder = WebApplication.CreateBuilder(args);

#region SignalR configuration
builder.Services.AddSignalR();
#endregion

#region OpenAPI/Swagger configuration
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
  var jwtSecurityScheme = new OpenApiSecurityScheme
  {
      Scheme = "bearer",
      BearerFormat = "JWT",
      Name = "Authorization",
      In = ParameterLocation.Header,
      Type = SecuritySchemeType.Http,
      Description = "Set only JWT without Barer prefix",

      Reference = new OpenApiReference
      {
          Id = JwtBearerDefaults.AuthenticationScheme,
          Type = ReferenceType.SecurityScheme
      }
  };
  c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
  c.AddSecurityRequirement(new OpenApiSecurityRequirement
  {
    { jwtSecurityScheme, Array.Empty<string>() }
  });
});
#endregion

#region JWT Token configuration
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton<IJwtTokenProvider, JwtTokenProvider>();

builder.Services.AddAuthentication("Bearer")
  .AddJwtBearer("Bearer", options =>
  {
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
    if (jwtSettings is null)
      throw new InvalidOperationException("JWT settings are not configured properly.");

    options.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuer = true,
      ValidIssuer = jwtSettings.Issuer,

      ValidateAudience = true,
      ValidAudience = jwtSettings.Audience,

      ValidateLifetime = true,

      RoleClaimType = ClaimTypes.Role,
      NameClaimType = ClaimTypes.Name,

      ValidateIssuerSigningKey = true,
      IssuerSigningKey = new SymmetricSecurityKey(
          Encoding.UTF8.GetBytes(jwtSettings.Secret)),
      ClockSkew = TimeSpan.Zero
    };
  });

builder.Services.AddAuthorization();
#endregion

#region CORS configuration
builder.Services.AddCors(options =>
{
  options.AddDefaultPolicy(policy =>
  {
    policy.WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
  });
});

#endregion

#region Infrastructure configuration
// Relational repositories
builder.Services.AddScoped<IAgentRepository, AgentRepository>();
builder.Services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
builder.Services.AddScoped<IProcessRepository, ProcessRepository>();
builder.Services.AddScoped<IPolicyRepository, PolicyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IPasswordHasher, HmacPasswordHasher>();

// SignalR notifier
builder.Services.AddScoped<IAgentStateNotifier, AgentNotifier>();
builder.Services.AddScoped<IAgentMetricNotifier, AgentNotifier>();

// Time-series repositories
builder.Services.AddScoped<IAgentMetricRepository, AgentMetricRepository>();
builder.Services.AddScoped<IAgentStateRepository, AgentStateRepository>();

// Long polling services
// builder.Services.AddSingleton<ILongPollingDispatcher<Guid, ConfigurationMessage>, InMemoryLongPollingDispatcher<Guid, ConfigurationMessage>>();
// builder.Services.AddSingleton<ILongPollingDispatcher<Guid, PoliciesMessage>, InMemoryLongPollingDispatcher<Guid, PoliciesMessage>>();
builder.Services.AddSingleton<ILongPollingDispatcher<Guid, ServerSyncMessage>, InMemoryLongPollingDispatcher<Guid, ServerSyncMessage>>();

// Postgres SQL database configuration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresSqlConnection")));

//Clickhouse client configuration
builder.Services.AddSingleton<ClickHouseConnection>(sp =>
{
  var connectionString = builder.Configuration.GetConnectionString("ClickHouseConnection");
  return new ClickHouseConnection(connectionString);
});
#endregion

#region Application configuration
builder.Services.AddScoped<IAgentService, AgentService>();
builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
builder.Services.AddScoped<IProcessService, ProcessService>();
builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAgentMetricService, AgentMetricService>();
builder.Services.AddScoped<IAgentStateService, AgentStateService>();
#endregion

var app = builder.Build();

#region Database migration
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	db.Database.Migrate();
}
#endregion

#region Development configuration
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "Server API V1");
		c.RoutePrefix = string.Empty;
	});
}
#endregion

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapAgentEndpoints();
app.MapAgentStateEndpoints();
app.MapAgentMetricEndpoints();
app.MapAuthEndpoints();
app.MapProcessEndpoints();
app.MapPolicyEndpoints();
app.MapConfigurationEndpoints();
app.MapUserEndpoints();
app.UseHttpsRedirection();

app.MapHub<AgentHub>("/agentHub");

app.Run();
