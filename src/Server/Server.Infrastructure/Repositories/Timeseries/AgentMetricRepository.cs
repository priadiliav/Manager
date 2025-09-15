using ClickHouse.Driver.ADO;
using ClickHouse.Driver.Utility;
using Server.Application.Abstractions;
using Server.Domain.Models;

namespace Server.Infrastructure.Repositories.TimeSeries;

public class AgentMetricRepository(ClickHouseConnection connection) : IAgentMetricRepository
{
  public async Task CreateAsync(AgentMetric agentMetric)
  {
    await using var command = connection.CreateCommand();

    command.CommandText =
    """
      INSERT INTO metrics (AgentId, Timestamp, CpuUsage, MemoryUsage, DiskUsage, NetworkUsage)
      VALUES (@AgentId, @Timestamp, @CpuUsage, @MemoryUsage, @DiskUsage, @NetworkUsage)
    """;

    command.AddParameter("AgentId", agentMetric.AgentId);
    command.AddParameter("Timestamp", agentMetric.Timestamp);
    command.AddParameter("CpuUsage", agentMetric.CpuUsage);
    command.AddParameter("MemoryUsage", agentMetric.MemoryUsage);
    command.AddParameter("DiskUsage", agentMetric.DiskUsage);
    command.AddParameter("NetworkUsage", agentMetric.NetworkUsage);

    await command.ExecuteNonQueryAsync();
  }

  public async Task<IEnumerable<AgentMetric>> GetAsync(
      Guid agentId,
      DateTimeOffset from,
      DateTimeOffset to,
      int limit = 50)
  {
    var metrics = new List<AgentMetric>();

    await using var command = connection.CreateCommand();

    command.CommandText =
    """
      SELECT AgentId, Timestamp, CpuUsage, MemoryUsage, DiskUsage, NetworkUsage
      FROM metrics
      WHERE AgentId = @AgentId AND Timestamp >= @From AND Timestamp <= @To
      ORDER BY Timestamp ASC
      LIMIT @Limit
    """;

    command.AddParameter("AgentId", agentId);
    command.AddParameter("From", from);
    command.AddParameter("To", to);
    command.AddParameter("Limit", limit);

    await using var reader = await command.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
      var metric = new AgentMetric
      {
          AgentId = reader.GetGuid(0),
          Timestamp = new DateTimeOffset(reader.GetDateTime(1), TimeSpan.Zero),
          CpuUsage = Convert.ToDouble(reader[2]),
          MemoryUsage = Convert.ToDouble(reader[3]),
          DiskUsage = Convert.ToDouble(reader[4]),
          NetworkUsage = Convert.ToDouble(reader[5]),
      };

      metrics.Add(metric);
    }

    return metrics;
  }
}
