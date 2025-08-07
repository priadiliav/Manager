using System.Text;
using Common.Messages.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Server.Api.Endpoints;
using Server.Application.Abstractions;
using Server.Application.Services;
using Server.Infrastructure.Communication;
using Server.Infrastructure.Configs;
using Server.Infrastructure.Repositories;
using Server.Infrastructure.Utils;

var builder = WebApplication.CreateBuilder(args);

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


// Database configuration
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Token configuration
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton<IJwtTokenProvider, JwtTokenProvider>();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
      var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = true,
          ValidIssuer = jwtSettings.Issuer,

          ValidateAudience = true,
          ValidAudience = jwtSettings.Audience,

          ValidateLifetime = true,

          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(
              Encoding.UTF8.GetBytes(jwtSettings.Secret)),
          ClockSkew = TimeSpan.Zero
      };
    });
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
// Repositories
builder.Services.AddScoped<IAgentRepository, AgentRepository>();
builder.Services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
builder.Services.AddScoped<IProcessRepository, ProcessRepository>();
builder.Services.AddScoped<IPolicyRepository, PolicyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services
builder.Services.AddScoped<IAgentService, AgentService>();
builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
builder.Services.AddScoped<IProcessService, ProcessService>();
builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddScoped<IUserService, UserService>();

// Long polling services
builder.Services.AddSingleton<ILongPollingDispatcher<long, ConfigurationMessage>, InMemoryLongPollingDispatcher<long, ConfigurationMessage>>();

var app = builder.Build();
// Migrate the database before starting the application
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	db.Database.Migrate();
}

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

// Map endpoints
app.MapAgentEndpoints();
app.MapProcessEndpoints();
app.MapPolicyEndpoints();
app.MapConfigurationEndpoints();
app.MapMetricEndpoints();
app.MapUserEndpoints();

app.UseHttpsRedirection();
app.Run();
