using Agent.WindowsService.Config;
using Agent.WindowsService.Domain;

namespace Agent.WindowsService.Application;

public partial class StateMachine
{
  private async Task HandleAuthenticationEntryAsync()
  {
    _logger.LogInformation("Entering Authentication state");

    try
    {
      var secret = await _secretStore.GetAsync(SecretConfig.ClientSecretKey);
      var refreshToken = await _secretStore.GetAsync(SecretConfig.RefreshTokenKey);
      var authToken = await _secretStore.GetAsync(SecretConfig.AuthTokenKey);

      _logger.LogInformation("Authenticating with server using AuthToken: {AuthToken}, RefreshToken: {RefreshToken}, ClientSecret: {ClientSecret}",
        authToken, refreshToken, secret);

      await _secretStore.SetAsync(SecretConfig.AuthTokenKey, new byte[] { 1, 2, 3, 4 }, CancellationToken.None);
      await _secretStore.SetAsync(SecretConfig.RefreshTokenKey, new byte[] { 5, 6, 7, 8 }, CancellationToken.None);
      await _secretStore.SetAsync(SecretConfig.ClientSecretKey, new byte[] { 9, 10, 11, 12 }, CancellationToken.None);

      // Simulate authentication delay
      await Task.Delay(500, CancellationToken.None);
      await _machine.FireAsync(Triggers.Success);

    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error in Authentication");
      await _machine.FireAsync(Triggers.Failed);
    }
  }

  private Task HandleAuthenticationExitAsync()
  {
    _logger.LogInformation("Exiting Authentication state");

    return Task.CompletedTask;
  }
}
