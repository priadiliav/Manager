import { useState, useEffect } from "react";
import { Box, Button, TextField, Autocomplete } from "@mui/material";
import { AgentCreateRequest, AgentDetailedDto } from "../../types/agent";
import { ConfigurationDto } from "../../types/configuration";
import { fetchConfigurations } from "../../api/configuration";

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
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        onSubmit(formData);
    };

    return (
        <Box component="form" onSubmit={handleSubmit} sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
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
                onChange={(event, newValue) => setFormData({ ...formData, configurationId: newValue?.id || "" })}
                renderInput={(params) => <TextField {...params} label="Configuration" required />}
            />
            <Button type="submit" variant="contained" color="primary">
                {mode === "create" ? "Create Agent" : "Save Changes"}
            </Button>
        </Box>
    );
};
