import { useState, useEffect } from "react";
import { Box, TextField, Autocomplete } from "@mui/material";
import { PolicyCreateRequest, PolicyDto, RegistryKeyType, RegistryValueType } from "../../types/policy";

type PolicyFormProps = {
    initialData: PolicyCreateRequest | PolicyDto;
    mode: "create" | "edit";
    onChange: (data: PolicyCreateRequest) => void;
};

const keyTypeOptions = Object.values(RegistryKeyType).filter((v) => typeof v === "number") as RegistryKeyType[];
const valueTypeOptions = Object.values(RegistryValueType).filter((v) => typeof v === "number") as RegistryValueType[];

export const PolicyForm = ({ initialData, mode, onChange }: PolicyFormProps) => {
    const [formData, setFormData] = useState<PolicyCreateRequest>({
        name: initialData.name ?? "",
        description: initialData.description ?? "",
        registryPath: initialData.registryPath ?? "",
        registryKey: initialData.registryKey ?? "",
        registryKeyType: initialData.registryKeyType ?? RegistryKeyType.Hklm,
        registryValueType: initialData.registryValueType ?? RegistryValueType.Qword,
    });

    useEffect(() => {
        onChange(formData);
    }, [formData, onChange]);

    const handleChange = (field: keyof PolicyCreateRequest, value: any) => {
        setFormData((prev) => ({ ...prev, [field]: value }));
    };

    return (
        <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
            <TextField
                size="small"
                label="Policy Name"
                value={formData.name}
                onChange={(e) => handleChange("name", e.target.value)}
                required
            />
            <TextField
                size="small"
                label="Description"
                value={formData.description}
                onChange={(e) => handleChange("description", e.target.value)}
                multiline
                rows={3}
            />
            <TextField
                size="small"
                label="Registry Path"
                value={formData.registryPath}
                onChange={(e) => handleChange("registryPath", e.target.value)}
                required
            />
            <TextField
                size="small"
                label="Registry Key"
                value={formData.registryKey}
                onChange={(e) => handleChange("registryKey", e.target.value)}
                required
            />
            <Autocomplete
                size="small"
                options={keyTypeOptions}
                getOptionLabel={(option) => RegistryKeyType[option]}
                value={formData.registryKeyType}
                onChange={(_, newValue) => handleChange("registryKeyType", newValue ?? RegistryKeyType.Hklm)}
                renderInput={(params) => <TextField {...params} label="Registry Key Type" />}
            />
            <Autocomplete
                size="small"
                options={valueTypeOptions}
                getOptionLabel={(option) => RegistryValueType[option]}
                value={formData.registryValueType}
                onChange={(_, newValue) => handleChange("registryValueType", newValue ?? RegistryValueType.Qword)}
                renderInput={(params) => <TextField {...params} label="Registry Value Type" />}
            />
        </Box>
    );
};
