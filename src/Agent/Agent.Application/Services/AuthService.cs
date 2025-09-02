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

public class AuthService(AgentStateContext context, HttpClient httpClient) : IAuthenticationService
{
  public async Task AuthenticateAsync(CancellationToken cancellationToken)
  {
    var loginRequest = new AgentLoginRequestMessage
    {
        AgentId = Guid.Parse("019906dd-c448-797d-99c2-b037a8caed09"),
        Secret = "a1f93576-2987-4c6d-9cdd-e7c7b0ac39f1"
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
