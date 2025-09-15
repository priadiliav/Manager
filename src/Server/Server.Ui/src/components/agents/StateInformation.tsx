import { useEffect, useState } from "react";
import { CustomTable } from "../table/CustomTable";
import { CollapsibleSection } from "../wrappers/CollapsibleSection";
import { useNavigate } from "react-router-dom";
import { SignalRClient } from "../../api/signalRClient";
import { AgentStateDto } from "../../types/state";

interface Props {
    agentId?: string;
    signalRClient?: SignalRClient
}

const columns = [
    { id: 'machine', label: 'Machine', minWidth: 100 },
    { id: 'fromState', label: 'From State', minWidth: 100 },
    { id: 'toState', label: 'To State', minWidth: 100 },
    { id: 'trigger', label: 'Trigger', minWidth: 100 },
    { id: 'timestamp', label: 'Timestamp', minWidth: 170 },
];

export const StateInformation = ({ agentId, signalRClient }: Props) => {
    const navigate = useNavigate();
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(10);
    const [rows, setRows] = useState<AgentStateDto[]>([]);

    useEffect(() => {
        if (!signalRClient) return;

        signalRClient.on("ReceiveAgentState", handleStateChange);

        return () => signalRClient.off("ReceiveAgentState", handleStateChange);
    }, [signalRClient]);


    //#region Handlers
    const handleChangePage = (_: unknown, newPage: number) => setPage(newPage);
    const handleStateChange = (message: AgentStateDto) => setRows(prev => [message, ...prev]);
    const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
        setRowsPerPage(parseInt(event.target.value, 10));
        setPage(0);
    };
    //#endregion

    return (
        <CollapsibleSection title="State Information">
            <CustomTable
                columns={columns}
                rows={rows}
                page={page}
                rowsPerPage={rowsPerPage}
                handleChangePage={handleChangePage}
                handleChangeRowsPerPage={handleChangeRowsPerPage}
                onRowClick={(row) => navigate(`/configurations/${row.id}`)}
            />
        </CollapsibleSection>
    );
}