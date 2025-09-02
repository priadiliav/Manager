using System.Net.NetworkInformation;
using Agent.Application.Abstractions;

namespace Agent.Infrastructure.Collectors;

public class NetworkUsageCollector(string interfaceName = null) : IDynamicDataCollector<double>
{
  public string Name => "network_usage";

  private long _lastBytesSent = 0;
  private long _lastBytesReceived = 0;
  private DateTime _lastTime = DateTime.UtcNow;

  public double Collect(CancellationToken cancellationToken = default)
  {
    var interfaces = NetworkInterface.GetAllNetworkInterfaces()
        .Where(ni => ni.OperationalStatus == OperationalStatus.Up);

    if (!string.IsNullOrEmpty(interfaceName))
      interfaces = interfaces.Where(ni => ni.Name == interfaceName);

    var niSelected = interfaces.FirstOrDefault();
    if (niSelected == null) return 0;

    var stats = niSelected.GetIPv4Statistics();
    var bytesSent = stats.BytesSent;
    var bytesReceived = stats.BytesReceived;

    var now = DateTime.UtcNow;
    var seconds = (now - _lastTime).TotalSeconds;

    var sentPerSec = (_lastBytesSent > 0) ? (bytesSent - _lastBytesSent) / seconds : 0;
    var receivedPerSec = (_lastBytesReceived > 0) ? (bytesReceived - _lastBytesReceived) / seconds : 0;

    _lastBytesSent = bytesSent;
    _lastBytesReceived = bytesReceived;
    _lastTime = now;

    return sentPerSec + receivedPerSec;
  }
}
