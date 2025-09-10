import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { createConfiguration, fetchConfigurationById } from "../../api/configuration";
import { ConfigurationDto, ConfigurationCreateRequest } from "../../types/configuration";
import FetchContentWrapper from "../../components/wrappers/FetchContentWrapper";
import { ConfigurationForm } from "../../components/configurations/ConfigurationForm";
import { Grid } from "@mui/material";

export const ConfigurationPage = () => {
    const navigate = useNavigate();
    const { id } = useParams<{ id: string }>();
    const isEdit = id !== undefined && id !== "new";

    const [loading, setLoading] = useState(isEdit);
    const [error, setError] = useState<string | null>(null);
    const [configuration, setConfiguration] = useState<ConfigurationDto | null>(null);

    useEffect(() => {
        if (isEdit && id) {
            const loadConfig = async () => {
                try {
                    const data = await fetchConfigurationById(id);
                    setConfiguration(data);
                } catch (err) {
                    console.error("Failed to fetch configuration", err);
                    setError("Failed to load configuration");
                } finally {
                    setLoading(false);
                }
            };
            loadConfig();
        }
    }, [id, isEdit]);

    const handleSubmit = async (data: ConfigurationCreateRequest) => {
        try {
            if (isEdit && id) {
                // await updateConfiguration(id, data);
            } else {
                await createConfiguration(data);
            }
            navigate("/configurations");
        } catch (err) {
            console.error("Failed to save configuration", err);
            setError("Failed to save configuration");
        }
    };

    return (
        <FetchContentWrapper loading={loading} error={error}>
            <Grid container spacing={2}>
                <Grid size={{ xs: 6, md: 4 }}>
                    <ConfigurationForm
                        initialData={configuration || { name: "" }}
                        mode={isEdit ? "edit" : "create"}
                        onSubmit={handleSubmit}
                    />
                </Grid>
            </Grid>
        </FetchContentWrapper>
    );
};
