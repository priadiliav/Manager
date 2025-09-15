import { useEffect, useState } from "react";
import { CustomTable } from "../table/CustomTable";
import { CollapsibleSection } from "../wrappers/CollapsibleSection";
import { useNavigate } from "react-router-dom";
import { SignalRClient } from "../../api/signalRClient";
import { AgentStateDto, AgentStateNodeDto } from "../../types/state";
import { StateTemplateTree } from "./StateTemplateTree";
import { fetchAgentStates, fetchAgentStateTemplateTree } from "../../api/states";
import FetchContentWrapper from "../wrappers/FetchContentWrapper";
import { Grid } from "@mui/material";

interface Props {
    agentId?: string;
    signalRClient?: SignalRClient
}

const columns = [
    { id: 'fromState', label: 'From State', minWidth: 100 },
    { id: 'toState', label: 'To State', minWidth: 100 },
    { id: 'trigger', label: 'Trigger', minWidth: 100 },
];

export const StateInformation = ({ agentId, signalRClient }: Props) => {
    const navigate = useNavigate();
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(10);
    const [rows, setRows] = useState<AgentStateDto[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [template, setTemplate] = useState<AgentStateNodeDto | null>(null);
    const [activeOverallState, setActiveOverallState] = useState<AgentStateDto | null>(null);

    useEffect(() => {
        if (!signalRClient) return;

        signalRClient.on("ReceiveAgentState", handleStateChange);

        return () => signalRClient.off("ReceiveAgentState", handleStateChange);
    }, [signalRClient]);

    useEffect(() => {
        const loadStateTemplate = async () => {
            try {
                setLoading(true);
                const template = await fetchAgentStateTemplateTree();
                setTemplate(template);
            } catch (error) {
                console.error("Failed to fetch agent state template tree:", error);
                setError("Failed to load state template tree.");
            } finally {
                setLoading(false);
            }
        }
        const fetchData = async () => {
            if (!agentId) return;
            try {
                setLoading(true);
                const states = await fetchAgentStates(agentId, new Date(0).toISOString(), new Date().toISOString(), 50);
                setRows(states);
                const lastOverallState = states.find(s => s.machine === "AgentOverallState");
                handleSetActiveOverallState(lastOverallState || null);
            } catch (error) {
                console.error("Failed to fetch agent states:", error);
                setError("Failed to load agent states.");
            } finally {
                setLoading(false);
            }
        }

        loadStateTemplate();
        fetchData();
    }, [agentId]);

    //#region Handlers
    const handleChangePage = (_: unknown, newPage: number) => setPage(newPage);
    const handleStateChange = (message: AgentStateDto) => {
        setRows(prev => [message, ...prev]);
        handleSetActiveOverallState(message);
    }
    const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
        setRowsPerPage(parseInt(event.target.value, 10));
        setPage(0);
    };

    const handleSetActiveOverallState = (state: AgentStateDto | null) => {
        if (state?.machine !== "AgentOverallState") return; //todo: temporary solution only for overall state
        setActiveOverallState(state);
    }
    //#endregion

    return (
        <FetchContentWrapper loading={loading} error={error}>
            <CollapsibleSection title="State Information">
                <Grid container spacing={2} >
                    <Grid size={{ md: 4 }} >
                        {template &&
                            <StateTemplateTree
                                template={template}
                                activeState={activeOverallState}
                            />
                        }
                    </Grid>
                    <Grid size={{ md: 8 }}>
                        <CustomTable
                            columns={columns}
                            rows={rows}
                            page={page}
                            rowsPerPage={rowsPerPage}
                            handleChangePage={handleChangePage}
                            handleChangeRowsPerPage={handleChangeRowsPerPage}
                        />
                    </Grid>
                </Grid>
            </CollapsibleSection >
        </FetchContentWrapper>
    );
}