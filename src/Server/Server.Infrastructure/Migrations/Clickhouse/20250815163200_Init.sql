CREATE TABLE metrics
(
  AgentId UUID,
  Timestamp DateTime,
  CpuUsage Float32,
  MemoryUsage Float32,
  DiskUsage Float32,
  CpuCores UInt16,
  MemoryTotal UInt64,
  DiskTotal UInt64
) ENGINE = MergeTree
ORDER BY (AgentId, Timestamp);
