using System.Text;
using System.Text.Json;
using Agent.Domain.Context;
using Common.Messages.Agent;
using Microsoft.Extensions.Logging;

namespace Agent.Application.Services;

public interface IAuthenticationService
{
  Task AuthenticateAsync(CancellationToken cancellationToken);
}

public class AuthService(
  AgentStateContext context,
  HttpClient httpClient,
  ILogger<AuthService> logger) : IAuthenticationService
{
  public async Task AuthenticateAsync(CancellationToken cancellationToken)
  {
    var loginRequest = new AgentLoginRequestMessage
    {
        AgentId = Guid.Parse("0198a897-68c4-7b90-9907-1a19038cf619"),
        Secret = "929cdc79-e7b4-4adc-b06c-de8463b27eec"
    };

    var json = JsonSerializer.Serialize(loginRequest);
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    var response = await httpClient.PostAsync(
        "/api/auth/agent/login",
        content,
        cancellationToken);

    if (!response.IsSuccessStatusCode)
      throw new UnauthorizedAccessException();

    var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
    var loginResponse = JsonSerializer.Deserialize<AgentLoginResponseMessage>(
        responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    if (loginResponse is null)
      throw new UnauthorizedAccessException();

    context.AuthenticationToken = loginResponse.Token;
  }
}
