using Agent.Domain.Context;
using Microsoft.Extensions.Logging;

namespace Agent.Application.Services;

public interface IAuthenticationService
{
  Task AuthenticateAsync(CancellationToken cancellationToken);
}

public class AuthenticationService(ILogger<AuthenticationService> logger) : IAuthenticationService
{
  public Task AuthenticateAsync(CancellationToken cancellationToken)
  {
    logger.LogInformation("Starting authentication process...");

    return Task.CompletedTask;
  }
}
