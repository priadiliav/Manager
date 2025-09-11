import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { createPolicy, fetchPolicyById } from "../../api/policy";
import { PolicyCreateRequest, PolicyDto, RegistryKeyType, RegistryValueType } from "../../types/policy";
import FetchContentWrapper from "../../components/wrappers/FetchContentWrapper";
import { PolicyForm } from "../../components/policies/PolicyForm";
import { Button } from "@mui/material";

export const PolicyPage = () => {
    const navigate = useNavigate();
    const { id } = useParams<{ id: string }>();
    const isEdit = id !== undefined && id !== "new";

    const [loading, setLoading] = useState(isEdit);
    const [error, setError] = useState<string | null>(null);
    const [policy, setPolicy] = useState<PolicyDto | null>(null);
    const [formData, setFormData] = useState<PolicyCreateRequest>({
        name: "",
        description: "",
        registryPath: "",
        registryKey: "",
        registryKeyType: RegistryKeyType.Hklm,
        registryValueType: RegistryValueType.Qword,
    });

    useEffect(() => {
        if (isEdit && id) {
            const loadPolicy = async () => {
                try {
                    const data = await fetchPolicyById(id);
                    setPolicy(data);
                    setFormData({
                        name: data.name,
                        description: data.description,
                        registryPath: data.registryPath,
                        registryKey: data.registryKey,
                        registryKeyType: data.registryKeyType,
                        registryValueType: data.registryValueType,
                    });
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

    const handleSubmit = async () => {
        try {
            if (isEdit && id) {
                // await updatePolicy(id, formData);
                console.log("Update policy", id, formData);
            } else {
                await createPolicy(formData);
            }
            navigate("/policies");
        } catch (err) {
            console.error("Failed to save policy", err);
            setError("Failed to save policy");
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
                {isEdit ? "Save Changes" : "Create Policy"}
            </Button>

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
                onChange={setFormData}
            />
        </FetchContentWrapper>
    );
};
