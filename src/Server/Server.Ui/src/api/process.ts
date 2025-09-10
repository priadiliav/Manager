import api from './axios';
import { ProcessCreateRequest, ProcessDto } from "../types/process";

export const fetchProcesses = async (): Promise<ProcessDto[]> => {
  const response = await api.get<ProcessDto[]>('/processes');
  return response.data;
};

export const fetchProcessById = async (id: string): Promise<ProcessDto> => {
  const response = await api.get<ProcessDto>(`/processes/${id}`);
  return response.data;
}

export const createProcess = async (process: ProcessCreateRequest): Promise<ProcessDto> => {
  const response = await api.post<ProcessDto>('/processes', process);
  return response.data;
};