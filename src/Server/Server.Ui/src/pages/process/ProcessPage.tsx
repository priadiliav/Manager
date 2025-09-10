import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { createProcess, fetchProcessById /* , updateProcess */ } from "../../api/process";
import { ProcessCreateRequest, ProcessDto } from "../../types/process";
import FetchContentWrapper from "../../components/wrappers/FetchContentWrapper";
import { ProcessForm } from "../../components/processes/ProcessForm";

export const ProcessPage = () => {
    const navigate = useNavigate();
    const { id } = useParams<{ id: string }>();
    const isEdit = id !== undefined && id !== "new";

    const [loading, setLoading] = useState(isEdit);
    const [error, setError] = useState<string | null>(null);
    const [process, setProcess] = useState<ProcessDto | null>(null);

    useEffect(() => {
        if (isEdit && id) {
            const loadProcess = async () => {
                try {
                    const data = await fetchProcessById(id);
                    setProcess(data);
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

    const handleSubmit = async (data: ProcessCreateRequest) => {
        try {
            if (isEdit && id) {
                // await updateProcess(id, data);
            } else {
                await createProcess(data);
            }
            navigate("/processes");
        } catch (err) {
            console.error("Failed to save process", err);
            setError("Failed to save process");
        }
    };

    return (
        <FetchContentWrapper loading={loading} error={error}>
            <ProcessForm
                initialData={process || { name: "" }}
                mode={isEdit ? "edit" : "create"}
                onSubmit={handleSubmit}
            />
        </FetchContentWrapper>
    );
};
