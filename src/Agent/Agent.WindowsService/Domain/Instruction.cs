namespace Agent.WindowsService.Domain;

public enum InstructionType
{
  PolicyUpdate = 1,
  PolicyDelete = 2,
  PolicyAdd = 3,
  ShellCommand = 4,
}

public class Instruction
{
  /// <summary>
  /// Unique identifier for the instruction
  /// </summary>
  public Guid AssociativeId { get; set; }

  /// <summary>
  /// Type of instruction
  /// </summary>
  public required InstructionType Type { get; set;  }

  /// <summary>
  /// Payload containing instruction details
  /// </summary>
  public required IReadOnlyDictionary<string, string> Payload { get; set;  }
}

public class InstructionResult
{
  /// <summary>
  /// Identifier of the instruction
  /// </summary>
  public required Guid AssociativeId { get; set; }

  /// <summary>
  /// Success status of the instruction execution
  /// </summary>
  public required bool Success { get; set; }

  /// <summary>
  /// Message or output from the instruction execution
  /// </summary>
  public string? Output { get; set; }

  /// <summary>
  /// Message or output from the instruction execution
  /// </summary>
  public string? Error { get; set; }
}


