import { Box, Chip } from "@mui/material";
import { useEffect, useState } from "react";
import { SignalRClient } from "../../api/signalRClient";
import { AgentStateDto } from "../../types/state";
import { CollapsibleSection } from "../wrappers/CollapsibleSection";
import { SimpleTreeView } from "@mui/x-tree-view/SimpleTreeView";
import { TreeItem } from "@mui/x-tree-view/TreeItem";

// ✅ Імпортуємо іконки MUI
import DashboardOutlinedIcon from "@mui/icons-material/DashboardOutlined";
import LockOutlinedIcon from "@mui/icons-material/LockOutlined";
import SyncOutlinedIcon from "@mui/icons-material/SyncOutlined";
import SettingsOutlinedIcon from "@mui/icons-material/SettingsOutlined";
import CloudSyncOutlinedIcon from "@mui/icons-material/CloudSyncOutlined";
import ShowChartOutlinedIcon from "@mui/icons-material/ShowChartOutlined";

interface Props {
    signalRClient?: SignalRClient;
}

export const AgentStateInfo = ({ signalRClient }: Props) => {
    const [states, setStates] = useState<Record<string, AgentStateDto>>({});

    useEffect(() => {
        if (!signalRClient) return;

        const handleState = (state: AgentStateDto) => {
            setStates((prev) => ({
                ...prev,
                [state.machine]: state,
            }));
        };

        signalRClient.on("ReceiveAgentState", handleState);
        return () => signalRClient.off("ReceiveAgentState", handleState);
    }, [signalRClient]);

    const getChip = (machineName: string) => {
        const s = states[machineName];
        if (!s) return <Chip label="Unknown" size="small" color="default" />;

        const color =
            s.toState === "Running"
                ? "success"
                : s.toState === "Processing"
                    ? "warning"
                    : s.toState === "Stopping"
                        ? "default"
                        : s.toState === "Error"
                            ? "error"
                            : "info";

        return <Chip label={s.toState} color={color as any} size="small" />;
    };

    const renderNodeLabel = (
        Icon: React.ElementType,
        name: string,
        machine: string
    ) => (
        <Box
            sx={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
                width: "100%",
                py: 0.5,
            }}
        >
            <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                <Icon sx={{ fontSize: 18, color: "text.secondary" }} />
                <span>{name}</span>
            </Box>
            {getChip(machine)}
        </Box>
    );

    return (
        <CollapsibleSection title="Agent State Machines">
            <Box >
                <SimpleTreeView
                    sx={{
                        "& .MuiTreeItem-label": {
                            width: "100%",
                        },
                    }}
                >
                    <TreeItem
                        itemId="overall"
                        label={renderNodeLabel(DashboardOutlinedIcon, "Overall", "OverallStateMachine")}
                    >
                        <TreeItem
                            itemId="auth"
                            label={renderNodeLabel(LockOutlinedIcon, "Auth", "AuthStateMachine")}
                        />
                        <TreeItem
                            itemId="sync"
                            label={renderNodeLabel(SyncOutlinedIcon, "Sync", "SyncStateMachine")}
                        />
                        <TreeItem
                            itemId="supervisor"
                            label={renderNodeLabel(SettingsOutlinedIcon, "Supervisor", "SupervisorStateMachine")}
                        >
                            <TreeItem
                                itemId="syncWorker"
                                label={renderNodeLabel(CloudSyncOutlinedIcon, "Sync Worker", "SyncWorkerStateMachine")}
                            />
                            <TreeItem
                                itemId="metricWorker"
                                label={renderNodeLabel(ShowChartOutlinedIcon, "Metric Worker", "MetricWorkerStateMachine")}
                            />
                        </TreeItem>
                    </TreeItem>
                </SimpleTreeView>
            </Box>
        </CollapsibleSection>
    );
};
