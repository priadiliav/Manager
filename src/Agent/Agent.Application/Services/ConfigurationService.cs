using Agent.Application.Abstractions;
using Common.Messages.Configuration;
using Microsoft.Extensions.Logging;

namespace Agent.Application.Services;

public interface IConfigurationService : ILongPollingRunner
{

}

public class ConfigurationService : IConfigurationService
{
    private readonly ILogger<ConfigurationService> _logger;
    private readonly ILongPollingClient<ConfigurationMessage> _longPollingClient;

    public ConfigurationService(
      ILongPollingClient<ConfigurationMessage> longPollingClient,
      ILogger<ConfigurationService> logger)
    {
      _longPollingClient = longPollingClient ?? throw new ArgumentNullException(nameof(longPollingClient));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    private Task HandleConfigurationUpdateAsync(ConfigurationMessage message)
    {
        _logger.LogInformation("Received configuration update: {Name}", message.Name);

        // Handle the configuration update message here
        // This could involve updating local state, notifying other components, etc.
        return Task.CompletedTask;
    }


    // Starts listening for configuration updates using the long polling client.
    public Task StartListeningAsync (CancellationToken cancellationToken)
      => _longPollingClient.StartListeningAsync(HandleConfigurationUpdateAsync, cancellationToken);
}
