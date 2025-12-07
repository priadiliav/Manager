using System.Diagnostics;
using Agent.Application.Abstractions;

namespace Agent.Infrastructure.Providers;

public class WindowsPolicyProvider : IPolicyProvider
{
  /// <summary>
  /// Generates a RSOP report in XML format and encodes it in Base64.
  /// </summary>
  /// <returns></returns>
  public string GenerateRsopReport()
  {
    string path = Path.Combine(Path.GetTempPath(), "rsop.xml");

    var psi = new ProcessStartInfo
    {
      FileName = "cmd.exe",
      Arguments = $"/c gpresult /x \"{path}\"",
      RedirectStandardOutput = true,
      RedirectStandardError = true,
      UseShellExecute = false,
      CreateNoWindow = true
    };

    using var process = new Process { StartInfo = psi };
    process.Start();

    string output = process.StandardOutput.ReadToEnd();
    string error = process.StandardError.ReadToEnd();
    process.WaitForExit();

    if (!File.Exists(path))
      throw new Exception($"gpresult failed. ExitCode={process.ExitCode}\nOutput: {output}\nError: {error}");

    var data = File.ReadAllBytes(path);
    return Convert.ToBase64String(data);
  }
}
