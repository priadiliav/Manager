

export interface MetricDto {
    agentId: string;
    timestamp: string; // ISO 8601 format
    cpuUsage: number; // in percentage
    memoryUsage: number; // in MB
    diskUsage: number; // in MB
    networkUsage: number; // in KB/s
}