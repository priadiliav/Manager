import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { createAgent, fetchAgentById } from "../../api/agent";
import { AgentCreateRequest, AgentDetailedDto } from "../../types/agent";
import FetchContentWrapper from "../../components/wrappers/FetchContentWrapper";
import { AgentForm } from "../../components/agents/AgentForm";

export const AgentPage = () => {
    const navigate = useNavigate();
    const { id } = useParams<{ id: string }>();
    const isEdit = id !== undefined && id !== "new";

    const [loading, setLoading] = useState(isEdit);
    const [error, setError] = useState<string | null>(null);
    const [agent, setAgent] = useState<AgentDetailedDto | null>(null);

    useEffect(() => {
        if (isEdit && id) {
            const loadAgent = async () => {
                try {
                    const data = await fetchAgentById(id);
                    setAgent(data);
                } catch (err) {
                    console.error("Failed to load agent", err);
                    setError("Failed to load agent");
                } finally {
                    setLoading(false);
                }
            };
            loadAgent();
        }
    }, [id, isEdit]);

    const handleSubmit = async (data: AgentCreateRequest) => {
        try {
            if (isEdit && id) {
                // await updateAgent(id, data);
            } else {
                await createAgent(data);
            }
            navigate("/agents");
        } catch (err) {
            console.error("Failed to save agent", err);
            setError("Failed to save agent");
        }
    };

    return (
        <FetchContentWrapper loading={loading} error={error}>
            <AgentForm
                initialData={agent || { name: "", configurationId: "" }}
                mode={isEdit ? "edit" : "create"}
                onSubmit={handleSubmit}
            />
        </FetchContentWrapper>
    );
};
