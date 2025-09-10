import { useState } from "react";
import { Box, Button, TextField } from "@mui/material";
import { ProcessCreateRequest, ProcessDto } from "../../types/process";

type ProcessFormProps = {
    initialData: ProcessCreateRequest | ProcessDto;
    mode: "create" | "edit";
    onSubmit: (data: ProcessCreateRequest) => void;
};

export const ProcessForm = ({ initialData, mode, onSubmit }: ProcessFormProps) => {
    const [formData, setFormData] = useState<ProcessCreateRequest>({
        name: initialData.name ?? "",
    });

    const handleChange = (field: keyof ProcessCreateRequest, value: string) => {
        setFormData((prev) => ({ ...prev, [field]: value }));
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        onSubmit(formData);
    };

    return (
        <Box
            component="form"
            onSubmit={handleSubmit}
            sx={{ display: "flex", flexDirection: "column", gap: 2 }}
        >
            <TextField
                size="small"
                label="Process Name"
                value={formData.name}
                onChange={(e) => handleChange("name", e.target.value)}
                required
            />
            <Button type="submit" variant="contained" color="primary">
                {mode === "edit" ? "Save Changes" : "Create Process"}
            </Button>
        </Box>
    );
};
