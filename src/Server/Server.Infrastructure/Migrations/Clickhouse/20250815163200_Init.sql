-- Table for storing agent metrics
CREATE TABLE agent_metric
(
  AgentId UUID,
  Timestamp DateTime,
  CpuUsage Float32,
  MemoryUsage Float32,
  DiskUsage Float32,
  NetworkUsage UInt16
) ENGINE = MergeTree
ORDER BY (AgentId, Timestamp);

-- Table for storing agent state transitions
CREATE TABLE agent_state
(
  AgentId UUID,
  Timestamp DateTime,
  Machine String,
  FromState String,
  ToState String,
  Trigger String
) ENGINE = MergeTree
ORDER BY (AgentId, Timestamp);


-- Cleanup old data older than 30 days
DELETE FROM agent_state
WHERE Timestamp < '2025-09-16 14:00:01'
