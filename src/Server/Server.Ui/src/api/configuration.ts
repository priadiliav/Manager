import api from './axios';
import { ConfigurationCreateRequest, ConfigurationDetailedDto, ConfigurationDto, ConfigurationModifyRequest } from '../types/configuration';

export const fetchConfigurations = async (): Promise<ConfigurationDto[]> => {
    const response = await api.get<ConfigurationDto[]>('api/configurations');
    return response.data;
}

export const fetchConfigurationById = async (id: string): Promise<ConfigurationDetailedDto> => {
    const response = await api.get<ConfigurationDetailedDto>(`api/configurations/${id}`);
    return response.data;
}

export const createConfiguration = async (configuration: ConfigurationCreateRequest): Promise<ConfigurationDetailedDto> => {
    const response = await api.post<ConfigurationDetailedDto>('api/configurations', configuration);
    return response.data;
}

export const modifyConfiguration = async (id: string, configuration: ConfigurationModifyRequest): Promise<ConfigurationDetailedDto> => {
    const response = await api.put<ConfigurationDetailedDto>(`api/configurations/${id}`, configuration);
    return response.data;
}