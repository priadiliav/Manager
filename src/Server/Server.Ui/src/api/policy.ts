import api from './axios';
import { PolicyCreateRequest, PolicyDto, PolicyModifyRequest } from '../types/policy';

export const fetchPolicies = async (): Promise<PolicyDto[]> => {
  const response = await api.get<PolicyDto[]>('api/policies');
  return response.data;
}

export const fetchPolicyById = async (id: string): Promise<PolicyDto> => {
  const response = await api.get<PolicyDto>(`api/policies/${id}`);
  return response.data;
}

export const createPolicy = async (policy: PolicyCreateRequest): Promise<PolicyDto> => {
  const response = await api.post<PolicyDto>('api/policies', policy);
  return response.data;
};

export const modifyPolicy = async (id: string, policy: PolicyModifyRequest): Promise<PolicyDto> => {
  const response = await api.put<PolicyDto>(`api/policies/${id}`, policy);
  return response.data;
}