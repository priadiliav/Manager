import { useEffect, useState } from "react";
import { ProcessDto } from "../../types/process";
import { useNavigate } from "react-router-dom";
import { fetchProcesses } from "../../api/process";
import FetchContentWrapper from "../../components/wrappers/FetchContentWrapper";
import { CustomTable } from "../../components/table/CustomTable";
import { Button } from "@mui/material";

const columns = [
    { id: 'id', label: 'ID', minWidth: 100 },
    { id: 'name', label: 'Name', minWidth: 150 },
];

export const ProcessesPage = () => {
    const navigate = useNavigate();
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(10);
    const [rows, setRows] = useState<ProcessDto[]>([]);

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        loadProcesses();
    }, [page, rowsPerPage]);

    //#region API Calls
    const loadProcesses = async () => {
        try {
            setLoading(true);
            setError(null);
            const processes = await fetchProcesses();
            setRows(processes);
        } catch (err) {
            console.error("Failed to fetch processes", err);
            setError("Failed to load processes. Please try again.");
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
        <FetchContentWrapper loading={loading} error={error} onRetry={loadProcesses}>
            <Button
                variant="contained"
                color="primary"
                onClick={() => navigate("/processes/new")}
            >
                Add Process
            </Button>
            <CustomTable
                columns={columns}
                rows={rows}
                page={page}
                rowsPerPage={rowsPerPage}
                handleChangePage={handleChangePage}
                handleChangeRowsPerPage={handleChangeRowsPerPage}
                onRowClick={(row) => navigate(`/processes/${row.id}`)}
            />
        </FetchContentWrapper>
    );
}