import { Box, Button } from "@mui/material";
import { KeyValueTable } from "../table/KeyValueTable";
import { CollapsibleSection } from "../wrappers/CollapsibleSection";

interface Props {
    lastSyncAt?: string | null;
    lastUnsyncAt?: string | null;
    isSynchronized: boolean;
}

export const SynchronizationInformation = ({ lastSyncAt, lastUnsyncAt, isSynchronized }: Props) => {
    const rows = [
        { key: "Last Synchronizated At", value: lastSyncAt ? new Date(lastSyncAt).toLocaleString() : "N/A" },
        { key: "Last Unsynchronized At", value: lastUnsyncAt ? new Date(lastUnsyncAt).toLocaleString() : "N/A" },
        { key: "Is Synchronized", value: isSynchronized ? "Yes" : "No" },
    ]

    return (
        <CollapsibleSection title="Synchronization Information" notification={!isSynchronized}>
            <Box sx={{ display: "flex", flexDirection: "column", gap: 2, mt: 2 }}>
                <KeyValueTable rows={rows} />
                <Button variant={isSynchronized ? "outlined" : "contained"} color="primary">Synchronize Now</Button>
            </Box>
        </CollapsibleSection>
    );
};
