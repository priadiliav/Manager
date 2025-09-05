CREATE TABLE metrics
(
  AgentId UUID,
  Timestamp DateTime,
  CpuUsage Float32,
  MemoryUsage Float32,
  DiskUsage Float32,
  NetworkUsage UInt16
) ENGINE = MergeTree
ORDER BY (AgentId, Timestamp);

