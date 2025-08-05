using Server.Domain.Models;

namespace Server.Application.Dtos.Process;

public class ProcessInConfigurationDto
{
	public long ProcessId { get; init; }
	public ProcessState ProcessState { get; init; }
}