namespace Agent.WindowsService.Config;

public static class UrlConfig
{
  private const string Version = "v1";

  public static readonly string Metric = $"/api/{Version}/metrics";

}
