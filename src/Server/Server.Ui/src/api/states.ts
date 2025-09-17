import api from './axios';

import { AgentStateDto, AgentStateNodeDto } from '../types/state';

// todo: incorrect
export const fetchAgentStates = async (agentId: string, from: string, to: string, limit: number): Promise<AgentStateDto[]> => {
    const response = await api.get<AgentStateDto[]>(`api/states/${agentId}`, {
        params: { from, to, limit }
    });
    return response.data;
};

export const fetchAgentStateTemplateTree = async (): Promise<AgentStateNodeDto> => {
    const response = await api.get<AgentStateNodeDto>('api/states/template');
    return response.data;
};