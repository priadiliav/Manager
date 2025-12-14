namespace Agent.WindowsService.Domain;

public enum States
{
  Idle,
  Authentication,
  Synchronization,
  Running,
  Execution,
  Delaying,
  Error
}
