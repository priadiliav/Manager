import { MetricDto } from '../types/metric';
import api from './axios';

export const fetchMetrics = async (agentId: string, from: string, to: string): Promise<MetricDto[]> => {
    const response = await api.get<MetricDto[]>(`/metrics`, {
        params: { agentId, from, to }
    });
    return response.data;
}
