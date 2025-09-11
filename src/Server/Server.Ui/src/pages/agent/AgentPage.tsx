import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { createAgent, fetchAgentById } from "../../api/agent";
import { AgentCreateRequest, AgentDetailedDto } from "../../types/agent";
import FetchContentWrapper from "../../components/wrappers/FetchContentWrapper";
import { AgentForm } from "../../components/agents/AgentForm";
import { Button, Grid } from "@mui/material";
import { AgentCharts } from "../../components/agents/AgentCharts";
import { HardwareInformation } from "../../components/agents/HardwareInformation";

export const AgentPage = () => {
    const navigate = useNavigate();
    const { id } = useParams<{ id: string }>();
    const isEdit = id !== undefined && id !== "new";

    const [loading, setLoading] = useState(isEdit);
    const [error, setError] = useState<string | null>(null);
    const [agent, setAgent] = useState<AgentDetailedDto | null>(null);
    const [formData, setFormData] = useState<AgentCreateRequest>({
        name: "",
        configurationId: ""
    });

    useEffect(() => {
        if (isEdit && id) {
            const loadAgent = async () => {
                try {
                    const data = await fetchAgentById(id);
                    setAgent(data);
                    setFormData({ name: data.name, configurationId: data.configurationId });
                } catch (err) {
                    console.error("Failed to load agent", err);
                    setError("Failed to load agent");
                } finally {
                    setLoading(false);
                }
            };
            loadAgent();
        }
    }, [id, isEdit]);

    const handleSubmit = async () => {
        try {
            if (isEdit && id) {
                // await updateAgent(id, formData);
                console.log("Update agent", id, formData);
            } else {
                await createAgent(formData);
            }
            navigate("/agents");
        } catch (err) {
            console.error("Failed to save agent", err);
            setError("Failed to save agent");
        }
    };

    return (
        <FetchContentWrapper loading={loading} error={error}>
            <Button
                variant="contained"
                color="primary"
                size="small"
                sx={{ alignSelf: "flex-end", mb: 2 }}
                onClick={handleSubmit}
            >
                {isEdit ? "Save Changes" : "Create Agent"}
            </Button>

            <Grid container spacing={2}>
                <Grid size={{ xs: 12, md: 4 }}>
                    <Grid size={{ xs: 12 }}>
                        <AgentForm
                            initialData={agent || {
                                name: "",
                                configurationId: ""
                            }}
                            mode={isEdit ? "edit" : "create"}
                            onSubmit={setFormData}
                        />
                    </Grid>
                    <Grid size={{ xs: 12 }}>
                        <HardwareInformation
                            initialData={agent?.hardware || {
                                id: "",
                                cpuModel: "",
                                cpuCores: 0,
                                cpuSpeedGHz: 0,
                                cpuArchitecture: "",
                                gpuModel: "",
                                gpuMemoryMB: 0,
                                ramModel: "",
                                totalMemoryMB: 0,
                                diskModel: "",
                                totalDiskMB: 0,
                                agentId: ""
                            }}
                        />
                    </Grid>
                </Grid>
                <Grid size={{ xs: 12, md: 8 }}>
                    <AgentCharts
                        agentId={id || ""}
                    />
                </Grid>
            </Grid>
        </FetchContentWrapper>
    );
};
