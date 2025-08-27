using ClickHouse.Driver.ADO;
using ClickHouse.Driver.Utility;
using Server.Application.Abstractions;
using Server.Domain.Models;

namespace Server.Infrastructure.Repositories.TimeSeries;

public class MetricRepository(ClickHouseConnection connection) : IMetricRepository
{
  public async Task CreateAsync(Metric metric)
  {
    await using var command = connection.CreateCommand();

    command.CommandText =
      """
        INSERT INTO metrics (AgentId, Timestamp, CpuUsage, MemoryUsage, DiskUsage)
        VALUES (@AgentId, @Timestamp, @CpuUsage, @MemoryUsage, @DiskUsage)
      """;

    command.AddParameter("AgentId", metric.AgentId);
    command.AddParameter("Timestamp", metric.Timestamp);
    command.AddParameter("CpuUsage", metric.CpuUsage);
    command.AddParameter("MemoryUsage", metric.MemoryUsage);
    command.AddParameter("DiskUsage", metric.DiskUsage);

    await command.ExecuteNonQueryAsync();
  }
}
