import { useEffect, useState } from "react";
import { CustomTable } from "../table/CustomTable";
import { CollapsibleSection } from "../wrappers/CollapsibleSection";
import { useNavigate } from "react-router-dom";
import * as signalR from "@microsoft/signalr";
import { SignalRClient } from "../../api/signalRClient";


interface Props {
    agentId?: string;
}

const columns = [
    { id: 'machine', label: 'Machine', minWidth: 100 },
    { id: 'fromState', label: 'From State', minWidth: 100 },
    { id: 'toState', label: 'To State', minWidth: 100 },
    { id: 'trigger', label: 'Trigger', minWidth: 100 },
    { id: 'timestamp', label: 'Timestamp', minWidth: 170 },
];

export const StateInformation = ({ agentId }: Props) => {
    const navigate = useNavigate();
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(10);
    const [rows, setRows] = useState<any[]>([]);

    useEffect(() => {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5267/agentHub")
            .withAutomaticReconnect()
            .build();

        const startConnection = async () => {
            try {
                await connection.start();
                console.log("SignalR connected");

                if (agentId) {
                    await connection.invoke("SubscribeToAgent", agentId);
                    console.log("Subscribed to agent", agentId);
                }
            } catch (err) {
                console.error("SignalR connection error: ", err);
                setTimeout(startConnection, 2000);
            }
        };

        startConnection();

        connection.on("ReceiveAgentStateChange", (message: any) => {
            setRows(prev => [message, ...prev]);
        });

        return () => {
            if (agentId) {
                connection.invoke("UnsubscribeFromAgent", agentId).catch(() => { });
            }
            connection.stop();
        };
    }, [agentId]);



    //#region Handlers
    const handleChangePage = (_: unknown, newPage: number) => setPage(newPage);
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