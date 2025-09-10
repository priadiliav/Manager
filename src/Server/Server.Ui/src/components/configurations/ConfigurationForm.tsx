import { Box, Button, TextField } from "@mui/material";
import { useState } from "react";
import { ConfigurationCreateRequest, ConfigurationDetailedDto } from "../../types/configuration";

interface Props {
    initialData: ConfigurationDetailedDto | ConfigurationCreateRequest;
    mode: "create" | "edit";
    onSubmit: (data: ConfigurationCreateRequest) => void;
}

export const ConfigurationForm = ({ initialData, mode, onSubmit }: Props) => {
    const [formData, setFormData] = useState<ConfigurationCreateRequest>({
        name: initialData.name,
    });

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        onSubmit(formData);
    };

    return (
        <Box component="form" onSubmit={handleSubmit} sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
            <TextField
                size="small"
                name="name"
                label="Configuration Name"
                value={formData.name}
                onChange={handleChange}
                required
            />
            <Button type="submit" variant="contained" color="primary">
                {mode === "create" ? "Create" : "Save Changes"}
            </Button>
        </Box>
    );
};
