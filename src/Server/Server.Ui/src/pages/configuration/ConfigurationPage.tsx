import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { createConfiguration, fetchConfigurationById } from "../../api/configuration";
import { ConfigurationCreateRequest, ConfigurationModifyRequest, ConfigurationDetailedDto } from "../../types/configuration";
import FetchContentWrapper from "../../components/wrappers/FetchContentWrapper";
import { ConfigurationForm } from "../../components/configurations/ConfigurationForm";
import { Button } from "@mui/material";
import { PolicyDto } from "../../types/policy";
import { fetchProcesses } from "../../api/process";
import { ProcessDto } from "../../types/process";
import { fetchPolicies } from "../../api/policy";

export const ConfigurationPage = () => {
    const navigate = useNavigate();
    const { id } = useParams<{ id: string }>();
    const isEdit = id !== undefined && id !== "new";

    const [loading, setLoading] = useState(isEdit);
    const [error, setError] = useState<string | null>(null);

    const [policies, setPolicies] = useState<PolicyDto[]>([]);
    const [processes, setProcesses] = useState<ProcessDto[]>([]);

    const [configuration, setConfiguration] = useState<ConfigurationDetailedDto | null>(null);
    const [formData, setFormData] = useState<ConfigurationCreateRequest | ConfigurationModifyRequest>({
        name: "", policies: [], processes: []
    });

    useEffect(() => {
        const loadPoliciesAndProcesses = async () => {
            try {
                const [policiesData, processesData] = await Promise.all([fetchPolicies(), fetchProcesses()]);
                setPolicies(policiesData);
                setProcesses(processesData);
            } catch (err) {
                console.error("Failed to fetch policies or processes", err);
                setError("Failed to load policies or processes");
            }
        };

        if (isEdit && id) {
            const loadConfig = async () => {
                try {
                    const data = await fetchConfigurationById(id);
                    setConfiguration(data);
                    setFormData({
                        name: data.name,
                        policies: data.policies,
                        processes: data.processes
                    });
                } catch (err) {
                    console.error("Failed to fetch configuration", err);
                    setError("Failed to load configuration");
                } finally {
                    setLoading(false);
                }
            };
            loadConfig();
        }

        loadPoliciesAndProcesses();
    }, [id, isEdit]);

    const handleSubmit = async () => {
        try {
            if (isEdit && id) {
                // await updateConfiguration(id, formData);
                console.log("Update configuration", id, formData);
            } else {
                await createConfiguration(formData);
            }
            navigate("/configurations");
        } catch (err) {
            console.error("Failed to save configuration", err);
            setError("Failed to save configuration");
        }
    };

    return (
        <FetchContentWrapper loading={loading} error={error}>
            <ConfigurationForm
                initialData={configuration || { id: "", name: "", policies: [], processes: [] }}
                initialPolicies={policies}
                initialProcesses={processes}
                mode={isEdit ? "edit" : "create"}
                onChange={setFormData}
            />
            <Button
                variant="contained"
                color="primary"
                size="small"
                sx={{ mt: 2 }}
                onClick={handleSubmit}
            >
                {isEdit ? "Save Changes" : "Create"}
            </Button>
        </FetchContentWrapper>
    );
};
