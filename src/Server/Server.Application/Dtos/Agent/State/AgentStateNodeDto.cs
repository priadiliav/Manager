namespace Server.Application.Dtos.Agent.State;

public class AgentStateNodeDto
{
  public string Name { get; set; } = string.Empty;
  public List<string> Transitions { get; set; } = new();
  public List<AgentStateNodeDto> Machines { get; set; } = new();

  public double X { get; set; }
  public double Y { get; set; }
}
