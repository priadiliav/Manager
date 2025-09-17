using Server.Application.Dtos.Agent.State;

namespace Server.Application.Resources;

public static class AgentStateTree
{
  public static readonly AgentStateNodeDto TemplateTree = new()
  {
      Name = "OverallState",
      MachineType = "Overall",
      X = 100,
      Y = 50,
      Transitions = ["Idle"],
      Machines = new List<AgentStateNodeDto>
      {
          new AgentStateNodeDto
          {
              MachineType = "OverallState",
              Name = "Idle",
              X = 100,
              Y = 120,
              Transitions = new List<string> { "Authenticating" },
          },
          new AgentStateNodeDto
          {
              MachineType = "OverallState",
              Name = "Authenticating",
              X = 100,
              Y = 190,
              Transitions = new List<string> { "Synchronizing", "Error", "Stopping" },
              Machines = new List<AgentStateNodeDto>()
              {
                  new AgentStateNodeDto
                  {
                      MachineType = "AuthStateState",
                      Name = "Idle",
                      X = 270,
                      Y = 190,
                      Transitions = new List<string> { "Processing" }
                  },
                  new AgentStateNodeDto
                  {
                      MachineType = "AuthStateState",
                      Name = "Processing",
                      X = 440,
                      Y = 190,
                      Transitions = new List<string> { "Finishing", "Error" }
                  },
                  new AgentStateNodeDto
                  {
                      MachineType = "AuthState",
                      Name = "Finishing",
                      X = 610,
                      Y = 190,
                      Transitions = new List<string> { "Stopping" }
                  },
                  new AgentStateNodeDto
                  {
                      MachineType = "AuthState",
                      Name = "Stopping",
                      X = 780,
                      Y = 190,
                      Transitions = new List<string> { "Idle" }
                  },

                  new AgentStateNodeDto
                  {
                      MachineType = "AuthState",
                      Name = "Error",
                      X = 780,
                      Y = 190,
                      Transitions = new List<string> { "Processing" }
                  },
              }
          },
          new AgentStateNodeDto
          {
              MachineType = "Overall",
              Name = "Synchronizing",
              X = 100,
              Y = 260,
              Transitions = new List<string> { "Running", "Error", "Stopping" },
              Machines = new List<AgentStateNodeDto>()
              {
                  new AgentStateNodeDto
                  {
                      MachineType = "SyncState",
                      Name = "Idle",
                      X = 270,
                      Y = 260,
                      Transitions = new List<string> { "Processing" }
                  },
                  new AgentStateNodeDto
                  {
                      MachineType = "SyncState",
                      Name = "Processing",
                      X = 440,
                      Y = 260,
                      Transitions = new List<string> { "Finishing", "Error" }
                  },
                  new AgentStateNodeDto
                  {
                      MachineType = "SyncState",
                      Name = "Finishing",
                      X = 610,
                      Y = 260,
                      Transitions = new List<string> { "Stopping" }
                  },
                  new AgentStateNodeDto
                  {
                      MachineType = "SyncState",
                      Name = "Stopping",
                      X = 780,
                      Y = 260,
                      Transitions = new List<string> { "Idle" }
                  },

                  new AgentStateNodeDto
                  {
                      MachineType = "SyncState",
                      Name = "Error",
                      X = 780,
                      Y = 260,
                      Transitions = new List<string> { "Processing" }
                  },
              }
          },
          new AgentStateNodeDto
          {
              MachineType = "OverallState",
              Name = "Running",
              X = 100,
              Y = 330,
              Transitions = new List<string> { "Error", "Stopping" },
              Machines = new List<AgentStateNodeDto>()
              {
                  new AgentStateNodeDto
                  {
                      MachineType = "RunnerState",
                      Name = "Idle",
                      X = 270,
                      Y = 330,
                      Transitions = new List<string> { "Processing" }
                  },

                  new AgentStateNodeDto
                  {
                      MachineType = "RunnerState",
                      Name = "Processing",
                      X = 440,
                      Y = 330,
                      Transitions = new List<string> { "Finishing", "Error" }
                  },

                  new AgentStateNodeDto
                  {
                      MachineType = "RunnerState",
                      Name = "Finishing",
                      X = 610,
                      Y = 330,
                      Transitions = new List<string> { "Idle" }
                  },

                  new AgentStateNodeDto
                  {
                      MachineType = "RunnerState",
                      Name = "Error",
                      X = 780,
                      Y = 330,
                      Transitions = new List<string> { "Processing" }
                  },
              }
          },
          new AgentStateNodeDto
          {
              MachineType = "Overall",
              Name = "Stopping",
              X = 100,
              Y = 400,
              Transitions = new List<string> { "Idle" }
          },
          new AgentStateNodeDto
          {
              MachineType = "Overall",
              Name = "Error",
              X = 100,
              Y = 470,
              Transitions = new List<string>()
          }
      }
  };
}
