namespace Agent.WindowsService.Common;


public record MetricMessage(
  int Type,
  string Name,
  double Value,
  string Unit,
  DateTime TimestampUtc,
  Dictionary<string, object>? Metadata);

public record InstructionResultMessage(
  Guid AssociatedId,
  bool Success,
  string? Output,
  string? Error);

public record InstructionMessage(
  Guid AssociatedId,
  int Type,
  IReadOnlyDictionary<string, string> Payload);
