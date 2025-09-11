import { useState, useEffect } from "react";
import { Box, TextField } from "@mui/material";
import { ProcessCreateRequest, ProcessDto } from "../../types/process";

type ProcessFormProps = {
    initialData: ProcessCreateRequest | ProcessDto;
    mode: "create" | "edit";
    onChange: (data: ProcessCreateRequest) => void;
};

export const ProcessForm = ({ initialData, mode, onChange }: ProcessFormProps) => {
    const [formData, setFormData] = useState<ProcessCreateRequest>({
        name: initialData.name ?? "",
    });

    useEffect(() => {
        onChange(formData);
    }, [formData, onChange]);

    const handleChange = (field: keyof ProcessCreateRequest, value: string) => {
        setFormData((prev) => ({ ...prev, [field]: value }));
    };

    return (
        <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
            <TextField
                size="small"
                label="Process Name"
                value={formData.name}
                onChange={(e) => handleChange("name", e.target.value)}
                required
            />
        </Box>
    );
};
