using Common.Messages;
using Microsoft.EntityFrameworkCore;
using Server.Api.Endpoints;
using Server.Application.Abstractions;
using Server.Application.Dtos.Configuration;
using Server.Application.Services;
using Server.Infrastructure.Communication;
using Server.Infrastructure.Configs;
using Server.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//Repositories
builder.Services.AddScoped<IAgentRepository, AgentRepository>();
builder.Services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
builder.Services.AddScoped<IProcessRepository, ProcessRepository>();
builder.Services.AddScoped<IPolicyRepository, PolicyRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services
builder.Services.AddScoped<IAgentService, AgentService>();
builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
builder.Services.AddScoped<IProcessService, ProcessService>();
builder.Services.AddScoped<IPolicyService, PolicyService>();

//Long polling services
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

app.MapAgentEndpoints();
app.MapProcessEndpoints();
app.MapPolicyEndpoints();
app.MapConfigurationEndpoints();
app.MapMetricEndpoints();

app.UseHttpsRedirection();
app.Run();
