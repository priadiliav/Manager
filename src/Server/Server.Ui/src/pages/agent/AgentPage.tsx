import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { createAgent, fetchAgentById } from "../../api/agent";
import { AgentCreateRequest, AgentCreateResponse, AgentDetailedDto } from "../../types/agent";
import FetchContentWrapper from "../../components/wrappers/FetchContentWrapper";
import { AgentForm } from "../../components/agents/AgentForm";
import { Box, Button, Grid, IconButton, TextField, Typography } from "@mui/material";
import { AgentCharts } from "../../components/agents/AgentCharts";
import { HardwareInformation } from "../../components/agents/HardwareInformation";
import CustomDialog from "../../components/dialogs/CustomDialog";
import ContentCopyIcon from "@mui/icons-material/ContentCopy";
import { AgentStatusInfo } from "../../components/agents/AgentStatusInfo";
import { StateInformation } from "../../components/agents/StateInformation";
import { SignalRClient } from "../../api/signalRClient";

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

    //#region Effects
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
            signalRClient.invoke("UnsubscribeFromAgent", id).catch(() => { });
            signalRClient.stop();
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

    const handleCopySecret = () => {
        if (createdAgentResponse) {
            navigator.clipboard.writeText(createdAgentResponse.secret || "");
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
                                status={agent?.status || 0}
                                lastStatusChangeAt={agent?.lastStatusChangeAt || null}
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
                    <Grid size={{ xs: 12, md: 12 }}>
                        {isEdit && (
                            <StateInformation
                                agentId={id || ""}
                                signalRClient={signalRClient}
                            />
                        )}
                    </Grid>
                </Grid>

            </Grid>

            <CustomDialog
                open={secretWindowOpen}
                title="Agent Secret"
                onClose={handleSecretWindowClose}
                submitLabel="OK"
                onSubmit={handleSecretWindowClose}
            >
                <Typography gutterBottom>
                    Agent id
                </Typography>
                <TextField
                    fullWidth
                    value={createdAgentResponse?.secret || ""}
                    InputProps={{
                        readOnly: true,
                        value: createdAgentResponse?.id || ""
                    }}
                />
                <Typography gutterBottom>
                    Please copy this secret. You will not be able to see it again:
                </Typography>
                <TextField
                    fullWidth
                    value={createdAgentResponse?.secret || ""}
                    InputProps={{
                        readOnly: true,
                        endAdornment: (
                            <IconButton onClick={handleCopySecret}>
                                <ContentCopyIcon />
                            </IconButton>
                        ),
                    }}
                />
            </CustomDialog>
        </FetchContentWrapper>
    );
};
