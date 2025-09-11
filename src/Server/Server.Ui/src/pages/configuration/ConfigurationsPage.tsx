import { useNavigate } from "react-router-dom";
import FetchContentWrapper from "../../components/wrappers/FetchContentWrapper";
import { useEffect, useState } from "react";
import { ConfigurationDto } from "../../types/configuration";
import { fetchConfigurations } from "../../api/configuration";
import { CustomTable } from "../../components/table/CustomTable";
import { Button } from "@mui/material";

const columns = [
    { id: 'name', label: 'Name', minWidth: 170 },
];

export const ConfigurationsPage = () => {
    const navigate = useNavigate();
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(10);
    const [rows, setRows] = useState<ConfigurationDto[]>([]);

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        loadConfigurations();
    }, [page, rowsPerPage]);

    //#region API Calls
    const loadConfigurations = async () => {
        try {
            setLoading(true);
            setError(null);
            const configurations = await fetchConfigurations();
            setRows(configurations);
        } catch (err) {
            console.error("Failed to fetch configurations", err);
            setError("Failed to load configurations. Please try again.");
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
        <FetchContentWrapper loading={loading} error={error}>
            <Button
                size="small"
                variant="contained"
                color="primary"
                onClick={() => navigate("/configurations/new")}
            >
                Add Configuration
            </Button>
            <CustomTable
                columns={columns}
                rows={rows}
                page={page}
                rowsPerPage={rowsPerPage}
                handleChangePage={handleChangePage}
                handleChangeRowsPerPage={handleChangeRowsPerPage}
                onRowClick={(row) => navigate(`/configurations/${row.id}`)}
            />
        </FetchContentWrapper>
    );
}