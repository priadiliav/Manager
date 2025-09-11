import { MetricDto } from '../types/metric';
import api from './axios';

// from iso string, to iso string
export const fetchMetrics = async (agentId: string, from: string, to: string, limit: number): Promise<MetricDto[]> => {
    const response = await api.get<MetricDto[]>(`/metrics`, {
        params: { agentId, from, to, limit }
    });
    return response.data;
}
