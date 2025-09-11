import { useState, useEffect } from "react";
import { Box, TextField, Autocomplete, Typography, IconButton, Collapse, Card, CardContent } from "@mui/material";
import { AgentCreateRequest, AgentDetailedDto } from "../../types/agent";
import { ConfigurationDto } from "../../types/configuration";
import { fetchConfigurations } from "../../api/configuration";
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';

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
    const [generalOpen, setGeneralOpen] = useState(true);
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
        <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
            <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between" }}>
                <Typography variant="h6">
                    General Information
                </Typography>
                <IconButton size="small" onClick={() => setGeneralOpen(!generalOpen)}>
                    <ExpandMoreIcon
                        sx={{
                            transform: generalOpen ? 'rotate(180deg)' : 'rotate(0deg)',
                            transition: 'transform 0.3s'
                        }}
                    />
                </IconButton>
            </Box>
            <Collapse in={generalOpen} timeout="auto" unmountOnExit>
                <Card>
                    <CardContent>
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
                    </CardContent>
                </Card>
            </Collapse>
        </Box>
    );
};
