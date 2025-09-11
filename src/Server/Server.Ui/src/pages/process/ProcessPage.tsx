import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { createProcess, fetchProcessById /* , updateProcess */ } from "../../api/process";
import { ProcessCreateRequest, ProcessDto } from "../../types/process";
import FetchContentWrapper from "../../components/wrappers/FetchContentWrapper";
import { ProcessForm } from "../../components/processes/ProcessForm";
import { Button } from "@mui/material";

export const ProcessPage = () => {
    const navigate = useNavigate();
    const { id } = useParams<{ id: string }>();
    const isEdit = id !== undefined && id !== "new";

    const [loading, setLoading] = useState(isEdit);
    const [error, setError] = useState<string | null>(null);
    const [process, setProcess] = useState<ProcessDto | null>(null);
    const [formData, setFormData] = useState<ProcessCreateRequest>({ name: "" });

    useEffect(() => {
        if (isEdit && id) {
            const loadProcess = async () => {
                try {
                    const data = await fetchProcessById(id);
                    setProcess(data);
                    setFormData({ name: data.name });
                } catch (err) {
                    console.error("Failed to load process", err);
                    setError("Failed to load process");
                } finally {
                    setLoading(false);
                }
            };
            loadProcess();
        }
    }, [id, isEdit]);

    const handleSubmit = async () => {
        try {
            if (isEdit && id) {
                // await updateProcess(id, formData);
                console.log("Update process", id, formData);
            } else {
                await createProcess(formData);
            }
            navigate("/processes");
        } catch (err) {
            console.error("Failed to save process", err);
            setError("Failed to save process");
        }
    };

    return (
        <FetchContentWrapper loading={loading} error={error}>
            <Button
                variant="contained"
                color="primary"
                size="small"
                sx={{ alignSelf: "flex-end", mb: 2 }}
                onClick={handleSubmit}
            >
                {isEdit ? "Save Changes" : "Create Process"}
            </Button>

            <ProcessForm
                initialData={process || { name: "" }}
                mode={isEdit ? "edit" : "create"}
                onChange={setFormData}
            />
        </FetchContentWrapper>
    );
};
