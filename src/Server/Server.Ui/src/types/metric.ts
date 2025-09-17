export interface AgentMetricDto {
  agentId: string;
  timestamp: string; // ISO 8601 format
  cpuUsage: number; // in percentage
  memoryUsage: number; // in percentage
  diskUsage: number; // in percentage
  networkUsage: number; // in KB/s
}
