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

public class AuthenticationService(
    AgentStateContext context,
    HttpClient httpClient,
    ILogger<AuthenticationService> logger) : IAuthenticationService
{
  public async Task AuthenticateAsync(CancellationToken cancellationToken)
  {
    var loginRequest = new AgentLoginRequestMessage
    {
        AgentId = Guid.Parse("01988658-1a43-79a5-9720-a0e3ef4ba673"),
        Secret = "cdd89139-d7ce-4a5c-9b6c-d107d564ed4c"
    };

    var json = JsonSerializer.Serialize(loginRequest);
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    var response = await httpClient.PostAsync(
        "/api/agents/login",
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
