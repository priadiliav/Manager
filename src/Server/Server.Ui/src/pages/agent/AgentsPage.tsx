import { useNavigate } from "react-router-dom";
import { CustomTable } from "../../components/table/CustomTable";
import { useEffect, useState } from "react";
import { AgentDto, AgentState } from "../../types/agent";
import { fetchAgents } from "../../api/agent";
import FetchContentWrapper from "../../components/wrappers/FetchContentWrapper";
import { Button } from "@mui/material";

const columns = [
    { id: 'id', label: 'ID', minWidth: 100 },
    { id: 'name', label: 'Name', minWidth: 150 },
    {
        id: 'state',
        label: 'State',
        minWidth: 120,
        render: (row: AgentDto) => {
            switch (row.state) {
                case AgentState.Online: return <span style={{ color: 'green' }}>Online</span>;
                case AgentState.Offline: return <span style={{ color: 'red' }}>Offline</span>;
                default: return <span style={{ color: 'gray' }}>Unknown</span>;
            }
        }
    }
];

export const AgentsPage = () => {
    const navigate = useNavigate();
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(10);
    const [rows, setRows] = useState<AgentDto[]>([]);

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        loadAgents();
    }, [page, rowsPerPage]);

    //#region API Calls
    const loadAgents = async () => {
        try {
            setLoading(true);
            setError(null);
            const agents = await fetchAgents();
            setRows(agents);
        } catch (err) {
            console.error("Failed to fetch agents", err);
            setError("Failed to load agents. Please try again.");
        } finally {
            setLoading(false);
        }
    };
    //#endregion

    //#region Handlers
    const handleChangePage = (_: unknown, newPage: number) => setPage(newPage);
    const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
        setRowsPerPage(parseInt(event.target.value, 10));
        setPage(0);
    };
    //#endregion

    return (
        <FetchContentWrapper loading={loading} error={error} onRetry={loadAgents}>
            <Button
                size="small"
                variant="contained"
                color="primary"
                onClick={() => navigate("/agents/new")}
            >
                Add Agents
            </Button>
            <CustomTable
                columns={columns}
                rows={rows}
                page={page}
                rowsPerPage={rowsPerPage}
                handleChangePage={handleChangePage}
                handleChangeRowsPerPage={handleChangeRowsPerPage}
                onRowClick={(row) => navigate(`/agents/${row.id}`)}
            />
        </FetchContentWrapper >
    );
}