using System.Security.Claims;
using System.Text;
using Common.Messages.Configuration;
using Common.Messages.Policy;
using Common.Messages.Process;
using k8s;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Server.Api.Endpoints;
using Server.Application.Abstractions;
using Server.Application.Services;
using Server.Infrastructure.Communication;
using Server.Infrastructure.Configs;
using Server.Infrastructure.Managers;
using Server.Infrastructure.Repositories;
using Server.Infrastructure.Utils;

var builder = WebApplication.CreateBuilder(args);

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

#region Database configuration
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
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

#region Infrastructure configuration
// Repositories
builder.Services.AddScoped<IAgentRepository, AgentRepository>();
builder.Services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
builder.Services.AddScoped<IProcessRepository, ProcessRepository>();
builder.Services.AddScoped<IPolicyRepository, PolicyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IPasswordHasher, HmacPasswordHasher>();

// Kubernetes client configuration
builder.Services.AddSingleton<IKubernetes>(sp =>
{
  var config = KubernetesClientConfiguration.InClusterConfig();
  return new Kubernetes(config);
});
builder.Services.AddSingleton<IClusterManager, ClusterManager>();

// Long polling services
builder.Services.AddSingleton<ILongPollingDispatcher<Guid, ConfigurationMessage>, InMemoryLongPollingDispatcher<Guid, ConfigurationMessage>>();
builder.Services.AddSingleton<ILongPollingDispatcher<Guid, PoliciesMessage>, InMemoryLongPollingDispatcher<Guid, PoliciesMessage>>();
builder.Services.AddSingleton<ILongPollingDispatcher<Guid, ProcessesMessage>, InMemoryLongPollingDispatcher<Guid, ProcessesMessage>>();

#endregion

#region Application configuration
builder.Services.AddScoped<IAgentService, AgentService>();
builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
builder.Services.AddScoped<IProcessService, ProcessService>();
builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
#endregion

var app = builder.Build();

#region Database migration
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	db.Database.Migrate();
}
#endregion

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

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapAgentEndpoints();
app.MapProcessEndpoints();
app.MapPolicyEndpoints();
app.MapConfigurationEndpoints();
app.MapMetricEndpoints();
app.MapUserEndpoints();
app.MapClusterEndpoints();

app.UseHttpsRedirection();
app.Run();
