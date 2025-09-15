using ClickHouse.Driver.ADO;
using ClickHouse.Driver.Utility;
using Server.Application.Abstractions;
using Server.Domain.Models;

namespace Server.Infrastructure.Repositories.TimeSeries;

public class AgentStateRepository(ClickHouseConnection connection) : IAgentStateRepository
{
  public async Task CreateAsync(AgentState agentState)
  {
    await using var command = connection.CreateCommand();

    command.CommandText =
    """
      INSERT INTO agent_state (AgentId, Timestamp, Machine, FromState, ToState, Trigger)
      VALUES (@AgentId, @Timestamp, @Machine, @FromState, @ToState, @Trigger)
    """;
    command.AddParameter("AgentId", agentState.AgentId);
    command.AddParameter("Timestamp", agentState.Timestamp);
    command.AddParameter("Machine", agentState.Machine);
    command.AddParameter("FromState", agentState.FromState);
    command.AddParameter("ToState", agentState.ToState);
    command.AddParameter("Trigger", agentState.Trigger);

    await command.ExecuteNonQueryAsync();
  }

  public async Task<AgentState?> GetCurrentStateAsync(Guid agentId)
  {
    await using var command = connection.CreateCommand();

    command.CommandText =
    """
      SELECT AgentId, Timestamp, Machine, FromState, ToState, Trigger
      FROM agent_state
      WHERE AgentId = @AgentId
      ORDER BY Timestamp DESC
      LIMIT 1
    """;
    command.AddParameter("AgentId", agentId);

    await using var reader = await command.ExecuteReaderAsync();
    if (await reader.ReadAsync())
    {
      return new AgentState
      {
        AgentId = reader.GetGuid(0),
        Timestamp = reader.GetDateTime(1),
        Machine = reader.GetString(2),
        FromState = reader.GetString(3),
        ToState = reader.GetString(4),
        Trigger = reader.GetString(5)
      };
    }

    return null;

  }

  public async Task<IEnumerable<AgentState>> GetAsync(Guid agentId, DateTimeOffset from, DateTimeOffset to, int limit = 1000)
  {
    var agentStates = new List<AgentState>();

    await using var command = connection.CreateCommand();

    command.CommandText =
    """
      SELECT AgentId, Timestamp, Machine, FromState, ToState, Trigger
      FROM agent_state
      WHERE AgentId = @AgentId AND Timestamp BETWEEN @From AND @To
      ORDER BY Timestamp DESC
      LIMIT @Limit
    """;
    command.AddParameter("AgentId", agentId);
    command.AddParameter("From", from);
    command.AddParameter("To", to);
    command.AddParameter("Limit", limit);

    await using var reader = await command.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
      var agentState = new AgentState
      {
        AgentId = reader.GetGuid(0),
        Timestamp = new DateTimeOffset(reader.GetDateTime(1), TimeSpan.Zero),
        Machine = reader.GetString(2),
        FromState = reader.GetString(3),
        ToState = reader.GetString(4),
        Trigger = reader.GetString(5)
      };
      agentStates.Add(agentState);
    }

    return agentStates;
  }
}

