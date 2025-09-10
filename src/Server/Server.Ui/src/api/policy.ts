import api from './axios';
import { PolicyCreateRequest, PolicyDto } from '../types/policy';

export const fetchPolicies = async (): Promise<PolicyDto[]> => {
  const response = await api.get<PolicyDto[]>('/policies');
  return response.data;
}

export const fetchPolicyById = async (id: string): Promise<PolicyDto> => {
  const response = await api.get<PolicyDto>(`/policies/${id}`);
  return response.data;
}

export const createPolicy = async (policy: PolicyCreateRequest): Promise<PolicyDto> => {
  const response = await api.post<PolicyDto>('/policies', policy);
  return response.data;
};