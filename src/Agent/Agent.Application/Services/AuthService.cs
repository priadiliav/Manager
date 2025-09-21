using Agent.Application.Abstractions;
using Agent.Domain.Configs;
using Common.Messages.Agent.Login;
using Microsoft.Extensions.Options;

namespace Agent.Application.Services;

public interface IAuthService
{
  Task AuthenticateAsync();
}

public class AuthService(
    IAgentRepository agentRepository,
    IConfigurationRepository configurationRepository,
    ICommunicationClient communicationClient,
    IOptions<EndpointsConfig> endpointsConfig) : IAuthService
{
  public async Task AuthenticateAsync()
  {
    var agentContext = await agentRepository.GetAsync();
    var authResponse = await communicationClient.PostAsync<AgentLoginResponseMessage, AgentLoginRequestMessage>(
        url: endpointsConfig.Value.Login,
        authenticate: false,
        message: new AgentLoginRequestMessage
        {
            AgentId = agentContext.Id,
            Secret = agentContext.Secret
        });

    if (authResponse is null || string.IsNullOrWhiteSpace(authResponse.Token))
        throw new InvalidOperationException("Authentication failed: Invalid response from server.");

    agentContext.UpdateToken(authResponse.Token);
    await agentRepository.SaveAsync(agentContext);
  }
}
