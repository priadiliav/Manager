export interface HardwareDto {
  id: string;
  agentId: string;
  cpuModel: string;
  cpuCores: number;
  cpuSpeedGHz: number;
  cpuArchitecture: string;
  gpuModel: string;
  gpuMemoryMB: number;
  ramModel: string;
  totalMemoryMB: number;
  diskModel: string;
  totalDiskMB: number;
}
