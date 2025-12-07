import { Box, Chip, IconButton, Tooltip } from "@mui/material";
import { useEffect, useState } from "react";
import { SignalRClient } from "../../api/signalRClient";
import { AgentStateDto } from "../../types/state";
import { CollapsibleSection } from "../wrappers/CollapsibleSection";
import { SimpleTreeView } from "@mui/x-tree-view/SimpleTreeView";
import { TreeItem } from "@mui/x-tree-view/TreeItem";

import DashboardOutlinedIcon from "@mui/icons-material/DashboardOutlined";
import LockOutlinedIcon from "@mui/icons-material/LockOutlined";
import SyncOutlinedIcon from "@mui/icons-material/SyncOutlined";
import SettingsOutlinedIcon from "@mui/icons-material/SettingsOutlined";
import CloudSyncOutlinedIcon from "@mui/icons-material/CloudSyncOutlined";
import ShowChartOutlinedIcon from "@mui/icons-material/ShowChartOutlined";
import RestartAltIcon from "@mui/icons-material/RestartAlt";
import PauseCircleOutlineIcon from '@mui/icons-material/PauseCircleOutline';

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

    const handleRestart = (machineName: string) => {
        console.log(`Restarting ${machineName}`);
    };

    const handleStop = (machineName: string) => {
        console.log(`Stopping ${machineName}`);
    };

    const renderNodeLabel = (
        Icon: React.ElementType,
        name: string,
        machine: string,
        restartHandler?: () => void,
        stopHandler?: () => void
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
            <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                {getChip(machine)}
                {stopHandler && (
                    <Tooltip title="Stop">
                        <IconButton
                            size="small"
                            onClick={(e) => {
                                e.stopPropagation();
                                stopHandler();
                            }}
                            sx={{ padding: 0.5 }}
                        >
                            <PauseCircleOutlineIcon sx={{ fontSize: 18 }} />
                        </IconButton>
                    </Tooltip>
                )}
                {restartHandler && (
                    <Tooltip title="Restart">
                        <IconButton
                            size="small"
                            onClick={(e) => {
                                e.stopPropagation();
                                restartHandler();
                            }}
                            sx={{ padding: 0.5 }}
                        >
                            <RestartAltIcon sx={{ fontSize: 18 }} />
                        </IconButton>
                    </Tooltip>
                )}
            </Box>
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
                        label={renderNodeLabel(
                            DashboardOutlinedIcon,
                            "Overall",
                            "OverallStateMachine"
                        )}
                    >
                        <TreeItem
                            itemId="auth"
                            label={renderNodeLabel(
                                LockOutlinedIcon,
                                "Auth",
                                "AuthStateMachine"
                            )}
                        />
                        <TreeItem
                            itemId="sync"
                            label={renderNodeLabel(
                                SyncOutlinedIcon,
                                "Sync",
                                "SyncStateMachine"
                            )}
                        />
                        <TreeItem
                            itemId="supervisor"
                            label={renderNodeLabel(
                                SettingsOutlinedIcon,
                                "Supervisor",
                                "SupervisorStateMachine"
                            )}
                        >
                            <TreeItem
                                itemId="syncWorker"
                                label={renderNodeLabel(
                                    CloudSyncOutlinedIcon,
                                    "Sync Worker (Receiver)",
                                    "SyncWorkerStateMachine",
                                )}
                            />
                            <TreeItem
                                itemId="metricWorker"
                                label={renderNodeLabel(
                                    ShowChartOutlinedIcon,
                                    "Metric Worker (Publisher)",
                                    "MetricWorkerStateMachine",
                                )}
                            />
                        </TreeItem>
                    </TreeItem>
                </SimpleTreeView>
            </Box>
        </CollapsibleSection>
    );
};
