import { Box, Button } from "@mui/material";
import { KeyValueTable } from "../table/KeyValueTable";
import { CollapsibleSection } from "../wrappers/CollapsibleSection";
import { AgentStatus } from "../../types/agent";

interface Props {
    status: AgentStatus;
    lastStatusChangeAt: string | null;
    isOnline: boolean;
}

function getStatusText(status: AgentStatus): string {
    switch (status) {
        case AgentStatus.Ok:
            return "Ok";
        case AgentStatus.NotSynchronized:
            return "Not Synchronized";
        default:
            return "Unknown";
    }
}

export const AgentStatusInfo = ({ status, lastStatusChangeAt, isOnline }: Props) => {
    const rows = [
        { key: 'Is online', value: isOnline ? 'Yes' : 'No' },
        { key: "Status", value: getStatusText(status) },
        { key: "Last Status Change At", value: lastStatusChangeAt ? new Date(lastStatusChangeAt).toLocaleString() : "N/A" },
    ]

    return (
        <CollapsibleSection title="Agent Status Information">
            <Box sx={{ display: "flex", flexDirection: "column", gap: 2, mt: 2 }}>
                <KeyValueTable rows={rows} />
                {isOnline && status === AgentStatus.NotSynchronized &&
                    <Button variant="contained" color="primary">
                        Synchronize Now
                    </Button>
                }
            </Box>
        </CollapsibleSection>
    );
};
