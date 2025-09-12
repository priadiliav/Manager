import { useState, useEffect } from "react";
import { Box, TextField, Autocomplete } from "@mui/material";
import { AgentCreateRequest, AgentDetailedDto } from "../../types/agent";
import { ConfigurationDto } from "../../types/configuration";
import { fetchConfigurations } from "../../api/configuration";
import { CollapsibleSection } from "../wrappers/CollapsibleSection";

interface Props {
    initialData: AgentCreateRequest | AgentDetailedDto;
    mode: "create" | "edit";
    onSubmit: (data: AgentCreateRequest) => void;
}

export const AgentForm = ({ initialData, mode, onSubmit }: Props) => {
    const [formData, setFormData] = useState<AgentCreateRequest>({
        name: initialData.name,
        configurationId: initialData.configurationId
    });

    const [configurations, setConfigurations] = useState<ConfigurationDto[]>([]);

    useEffect(() => {
        const loadConfigs = async () => {
            try {
                const configs = await fetchConfigurations();
                setConfigurations(configs);
            } catch (err) {
                console.error("Failed to load configurations", err);
            }
        };
        loadConfigs();
    }, []);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const newData = { ...formData, [e.target.name]: e.target.value };
        setFormData(newData);
        onSubmit(newData);
    };

    return (
        <CollapsibleSection title="General Information">
            <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
                <TextField
                    size="small"
                    name="name"
                    label="Agent Name"
                    value={formData.name}
                    onChange={handleChange}
                    required
                />
                <Autocomplete
                    size="small"
                    options={configurations}
                    getOptionLabel={(option) => option.name}
                    value={configurations.find((c) => c.id === formData.configurationId) || null}
                    onChange={(event, newValue) => {
                        const newData = { ...formData, configurationId: newValue?.id || "" };
                        setFormData(newData);
                        onSubmit(newData);
                    }}
                    renderInput={(params) => <TextField {...params} label="Configuration" required />}
                />
            </Box>
        </CollapsibleSection>

    );
};
