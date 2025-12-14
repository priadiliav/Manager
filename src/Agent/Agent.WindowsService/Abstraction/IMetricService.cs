namespace Agent.WindowsService.Abstraction;

public interface IMetricService
{
  /// <summary>
  /// Send collected metrics to the server
  /// </summary>
  Task SendAsync(CancellationToken cancellationToken);
}
