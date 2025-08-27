CREATE TABLE metrics
(
  AgentId UUID,
  Timestamp DateTime,
  CpuUsage Float64,
  MemoryUsage Float64,
  DiskUsage Float64
)
ENGINE = MergeTree
PRIMARY KEY (AgentId, Timestamp)
ORDER BY (AgentId, Timestamp);
