using System.Diagnostics;
using Agent.WindowsService.Abstraction;
using Agent.WindowsService.Domain;

namespace Agent.WindowsService.Infrastructure.Executors;

public class ShellExecutor : IInstructionExecutor
{
  public bool CanExecute(InstructionType type) => type is InstructionType.ShellCommand;

  public async Task<InstructionResult> ExecuteAsync(Instruction instruction, CancellationToken cancellationToken = default)
  {
    var command = instruction.Payload["command"];

    using var process = new Process();
    process.StartInfo = new ProcessStartInfo
    {
      FileName = "cmd.exe",
      Arguments = $"/c {command}",
      UseShellExecute = false,
      RedirectStandardOutput = true,
      RedirectStandardError = true,
      CreateNoWindow = true,
      WorkingDirectory = Environment.SystemDirectory
    };

    var output = new System.Text.StringBuilder();
    var error = new System.Text.StringBuilder();

    process.OutputDataReceived += (_, e) => { if (e.Data != null) output.AppendLine(e.Data); };
    process.ErrorDataReceived += (_, e) => { if (e.Data != null) error.AppendLine(e.Data); };

    process.Start();
    process.BeginOutputReadLine();
    process.BeginErrorReadLine();

    // todo: make timeout configurable
    var timeout = 5000;
    var exited = await Task.Run(() => process.WaitForExit(timeout), cancellationToken);
    if (!exited)
    {
      try { process.Kill(entireProcessTree: true); } catch { }
      return new InstructionResult
      {
        AssociativeId = instruction.AssociativeId,
        Success = false,
        Output = output.ToString(),
        Error = "Process timed out"
      };
    }

    return new InstructionResult
    {
      AssociativeId = instruction.AssociativeId,
      Success = process.ExitCode == 0,
      Output = output.ToString(),
      Error = error.ToString()
    };
  }
}
