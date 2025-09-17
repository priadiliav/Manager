import { useEffect, useState } from "react";
import { CollapsibleSection } from "../wrappers/CollapsibleSection";
import { SignalRClient } from "../../api/signalRClient";
import { AgentStateDto, AgentStateNodeDto } from "../../types/state";
import { StateTemplateTree } from "./StateTemplateTree";
import { fetchAgentStates, fetchAgentStateTemplateTree } from "../../api/states";
import FetchContentWrapper from "../wrappers/FetchContentWrapper";

interface Props {
    agentId?: string;
    signalRClient?: SignalRClient
}

export const StateInformation = ({ agentId, signalRClient }: Props) => {
    const [rows, setRows] = useState<AgentStateDto[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [template, setTemplate] = useState<AgentStateNodeDto | null>(null);
    const [activeOverallState, setActiveOverallState] = useState<AgentStateDto | null>(null);

    useEffect(() => {
        if (!signalRClient) return;

        signalRClient.on("ReceiveAgentState", handleStateChange);

        return () => signalRClient.off("ReceiveAgentState", handleStateChange);

        // eslint-disable-next-line react-hooks/exhaustive-deps
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
                handleSetActiveOverallState(states[0]);
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
    const handleStateChange = (message: AgentStateDto) => {
        setRows(prev => [message, ...prev]);
        handleSetActiveOverallState(message);
    }
    const handleSetActiveOverallState = (state: AgentStateDto | null) => {
        console.log(state);

        setActiveOverallState(state);
    }
    //#endregion

    return (
        <FetchContentWrapper loading={loading} error={error}>
            <CollapsibleSection title="State Information">
                {template &&
                    <StateTemplateTree
                        template={template}
                        activeState={activeOverallState}
                    />
                }
            </CollapsibleSection >
        </FetchContentWrapper>
    );
}