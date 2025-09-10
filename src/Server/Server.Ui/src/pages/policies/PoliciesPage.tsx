import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { PolicyDto, RegistryKeyType, RegistryValueType } from "../../types/policy";
import { fetchPolicies } from "../../api/policy";
import FetchContentWrapper from "../../components/wrappers/FetchContentWrapper";
import { CustomTable } from "../../components/table/CustomTable";
import { Button } from "@mui/material";

const columns = [
    { id: 'id', label: 'ID', minWidth: 100 },
    { id: 'name', label: 'Name', minWidth: 150 },
    { id: 'description', label: 'Description', minWidth: 200 },
    { id: 'registryPath', label: 'Registry Path', minWidth: 200 },
    { id: 'registryKey', label: 'Registry Key', minWidth: 150 },
    { id: 'registryKeyType', label: 'Registry Key Type', minWidth: 150, render: (row: PolicyDto) => RegistryKeyType[row.registryKeyType] },
    { id: 'registryValueType', label: 'Registry Value Type', minWidth: 150, render: (row: PolicyDto) => RegistryValueType[row.registryValueType] }
];

export const PoliciesPage = () => {
    const navigate = useNavigate();
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(10);
    const [rows, setRows] = useState<PolicyDto[]>([]);

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        loadPolicies();
    }, [page, rowsPerPage]);

    //#region API Calls
    const loadPolicies = async () => {
        try {
            setLoading(true);
            setError(null);
            const policies = await fetchPolicies();
            setRows(policies);
        } catch (err) {
            console.error("Failed to fetch policies", err);
            setError("Failed to load policies. Please try again.");
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
        <FetchContentWrapper loading={loading} error={error} onRetry={loadPolicies}>
            <Button
                variant="contained"
                color="primary"
                onClick={() => navigate("/policies/new")}
            >
                Add Policy
            </Button>
            <CustomTable
                columns={columns}
                rows={rows}
                page={page}
                rowsPerPage={rowsPerPage}
                handleChangePage={handleChangePage}
                handleChangeRowsPerPage={handleChangeRowsPerPage}
                onRowClick={(row) => navigate(`/policies/${row.id}`)}
            />
        </FetchContentWrapper>
    );
}