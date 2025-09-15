using Server.Application.Dtos.Agent.State;

namespace Server.Application.Resources;

public static class AgentStateTree
{
  public static readonly AgentStateNodeDto TemplateTree = new()
  {
      Name = "Overall",
      X = 100,
      Y = 50,
      Transitions = ["Idle"],
      Machines = new List<AgentStateNodeDto>
      {
          new AgentStateNodeDto
          {
              Name = "Idle",
              X = 100,  // зсув праворуч
              Y = 100,
              Transitions = new List<string> { "Authenticating" },
          },
          new AgentStateNodeDto
          {
              Name = "Authenticating",
              X = 100,
              Y = 150,
              Transitions = new List<string> { "Synchronizing", "Error", "Stopping" }
          },
          new AgentStateNodeDto
          {
              Name = "Synchronizing",
              X = 100,
              Y = 200,
              Transitions = new List<string> { "Running", "Error", "Stopping" }
          },
          new AgentStateNodeDto
          {
              Name = "Running",
              X = 100,
              Y = 250,
              Transitions = new List<string> { "Error", "Stopping" }
          },
          new AgentStateNodeDto
          {
              Name = "Stopping",
              X = 100,
              Y = 300,
              Transitions = new List<string> { "Idle" }
          },
          new AgentStateNodeDto
          {
              Name = "Error",
              X = 100,
              Y = 350,
              Transitions = new List<string>()
          }
      }
  };
}
