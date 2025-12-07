import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { createAgent, fetchAgentById, notifySynchronization } from "../../api/agent";
import { AgentCreateRequest, AgentCreateResponse, AgentDetailedDto } from "../../types/agent";
import FetchContentWrapper from "../../components/wrappers/FetchContentWrapper";
import { AgentForm } from "../../components/agents/AgentForm";
import { Box, Button, Grid } from "@mui/material";
import { AgentCharts } from "../../components/agents/AgentCharts";
import { HardwareInformation } from "../../components/agents/HardwareInformation";
import { AgentStatusInfo } from "../../components/agents/AgentStatusInfo";
import { SignalRClient } from "../../api/signalRClient";
import { AgentCreatedDialog } from "../../components/agents/AgentCreatedDialog";
import { AgentStateInfo } from "../../components/agents/AgentStateInfo";

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

    const [secretWindowOpen, setSecretWindowOpen] = useState(false);
    const [createdAgentResponse, setCreatedAgentResponse] = useState<AgentCreateResponse | null>(null);
    const [signalRClient] = useState(() => new SignalRClient("agentHub"));

    const loadAgent = async () => {
        if (isEdit && id) {
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
        }
    };

    //#region Effects
    useEffect(() => {
        loadAgent();
    }, [id, isEdit]);

    useEffect(() => {
        if (!isEdit || !id) return;

        const startConnection = async () => {
            try {
                await signalRClient.start();
                console.log("SignalR connected");

                await signalRClient.invoke("SubscribeToAgent", id);
                console.log("Subscribed to agent", id);
            } catch (err) {
                console.error("SignalR start/invoke error:", err);
            }
        };

        startConnection();

        return () => {
            const cleanup = async () => {
                try {
                    await signalRClient.invoke("UnsubscribeFromAgent", id);
                    console.log("Unsubscribed from agent", id);
                } catch (err) {
                    console.warn("Unsubscribe failed:", err);
                } finally {
                    await signalRClient.stop();
                    console.log("SignalR stopped");
                }
            };

            cleanup();
        };
    }, [id, isEdit, signalRClient]);
    //#endregion

    //#region Handlers
    const handleSubmit = async () => {
        try {
            if (isEdit && id) {
                // await updateAgent(id, formData);
                console.log("Update agent", id, formData);
            } else {
                const createdAgent = await createAgent(formData);
                setCreatedAgentResponse(createdAgent);
                setSecretWindowOpen(true);
            }
        } catch (err) {
            console.error("Failed to save agent", err);
            setError("Failed to save agent");
        }
    };

    const handleSecretWindowClose = () => {
        setSecretWindowOpen(false);
        navigate(`/agents/${createdAgentResponse?.id}`);
    };


    const handleSynchronize = async () => {
        if (!id) return;

        try {
            await notifySynchronization(id);
            loadAgent();
            console.log("Synchronization initiated");
        } catch (err) {
            console.error("Failed to initiate synchronization", err);
        }
    };
    //#endregion

    return (
        <FetchContentWrapper loading={loading} error={error}>
            <Box display="flex" alignItems="center" gap={1} mb={1}>
                <Button
                    variant="contained"
                    color="primary"
                    size="small"
                    onClick={handleSubmit}
                >
                    {isEdit ? "Save Changes" : "Create Agent"}
                </Button>
            </Box>

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
                        {isEdit && (
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
                            />)}
                    </Grid>
                    <Grid size={{ xs: 12 }}>
                        {isEdit && (
                            <AgentStatusInfo
                                handleSynchronize={handleSynchronize}
                                status={agent?.status || 0}
                                lastStatusChangeAt={agent?.lastStatusChangeAt || null}
                            />
                        )}
                    </Grid>
                    <Grid size={{ xs: 12 }}>
                        {isEdit && (
                            <AgentStateInfo
                                signalRClient={signalRClient}
                            />
                        )}
                    </Grid>
                </Grid>
                <Grid size={{ xs: 12, md: 8 }}>
                    <Grid size={{ xs: 12, md: 12 }}>
                        {isEdit && (
                            <AgentCharts
                                agentId={id || ""}
                                signalRClient={signalRClient}
                            />)}
                    </Grid>
                </Grid>
            </Grid>

            <AgentCreatedDialog
                secretWindowOpen={secretWindowOpen}
                createdAgentResponse={createdAgentResponse}
                handleClose={handleSecretWindowClose}
            />
        </FetchContentWrapper>
    );
};
