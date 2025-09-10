import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { createPolicy, fetchPolicyById } from "../../api/policy";
import { PolicyCreateRequest, PolicyDto, RegistryKeyType, RegistryValueType } from "../../types/policy";
import FetchContentWrapper from "../../components/wrappers/FetchContentWrapper";
import { PolicyForm } from "../../components/policies/PolicyForm";

export const PolicyPage = () => {
    const navigate = useNavigate();
    const { id } = useParams<{ id: string }>();
    const isEdit = id !== undefined && id !== "new";

    const [loading, setLoading] = useState(isEdit);
    const [error, setError] = useState<string | null>(null);
    const [policy, setPolicy] = useState<PolicyDto | null>(null);

    useEffect(() => {
        if (isEdit && id) {
            const loadPolicy = async () => {
                try {
                    const data = await fetchPolicyById(id);
                    setPolicy(data);
                } catch (err) {
                    console.error("Failed to load policy", err);
                    setError("Failed to load policy");
                } finally {
                    setLoading(false);
                }
            };
            loadPolicy();
        }
    }, [id, isEdit]);

    const handleSubmit = async (data: PolicyCreateRequest) => {
        try {
            if (isEdit && id) {
                // await updatePolicy(id, data);
            } else {
                await createPolicy(data);
            }
            navigate("/policies");
        } catch (err) {
            console.error("Failed to save policy", err);
            setError("Failed to save policy");
        }
    };

    return (
        <FetchContentWrapper loading={loading} error={error}>
            <PolicyForm
                initialData={
                    policy || {
                        name: "",
                        description: "",
                        registryPath: "",
                        registryKey: "",
                        registryKeyType: RegistryKeyType.Hklm,
                        registryValueType: RegistryValueType.Qword,
                    }
                }
                mode={isEdit ? "edit" : "create"}
                onSubmit={handleSubmit}
            />
        </FetchContentWrapper>
    );
};
