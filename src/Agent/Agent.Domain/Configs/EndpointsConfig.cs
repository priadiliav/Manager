namespace Agent.Domain.Configs;

public class EndpointsConfig
{
  public string BaseUrl { get; set; } = string.Empty;

  public string Login { get; set; } = string.Empty;
  public string Metrics { get; set; } = string.Empty;
  public string Sync { get; set; } = string.Empty;
  public string SyncSubscribe { get; set; } = string.Empty;
}
