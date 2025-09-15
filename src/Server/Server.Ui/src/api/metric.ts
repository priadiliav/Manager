import { AgentMetricDto } from '../types/metric';
import api from './axios';

// from iso string, to iso string
export const fetchMetrics = async (agentId: string, from: string, to: string, limit: number): Promise<AgentMetricDto[]> => {
    const response = await api.get<AgentMetricDto[]>(`api/metrics`, {
        params: { agentId, from, to, limit }
    });
    return response.data;
}
