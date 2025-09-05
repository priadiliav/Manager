import { AgentDto, AgentCreateRequest, AgentCreateResponse, AgentModifyRequest, AgentDetailedDto } from '../types/agent';
import api from './axios';

export const fetchAgents = async (): Promise<AgentDto[]> => {
  const response = await api.get<AgentDto[]>('/agents');
  return response.data;
};

export const fetchAgentById = async (id: string): Promise<AgentDetailedDto> => {
  const response = await api.get<AgentDetailedDto>(`/agents/${id}`);
  return response.data;
}

export const createAgent = async (agent: AgentCreateRequest): Promise<AgentCreateResponse> => {
  const response = await api.post<AgentCreateResponse>('/agents', agent);
  return response.data;
};

export const modifyAgent = async (id: string, agent: AgentModifyRequest): Promise<AgentDto> => {
  const response = await api.put<AgentDto>(`/agents/${id}`, agent);
  return response.data;
};

export const deleteAgent = async (id: string): Promise<void> => {
  await api.delete<void>(`/agents/${id}`);
};
