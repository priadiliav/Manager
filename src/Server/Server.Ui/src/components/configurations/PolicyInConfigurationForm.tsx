import { Autocomplete, Box, TextField } from "@mui/material";
import { PolicyDto, PolicyInConfigurationDto } from "../../types/policy";

interface Props {
    policy: PolicyInConfigurationDto;
    index: number;
    allPolicies: PolicyDto[];
    onPolicyChange: (index: number, newPolicy: PolicyDto | null) => void;
    onRegistryValueChange: (index: number, value: string) => void;
}

export const PolicyInConfigurationForm = ({
    policy,
    index,
    allPolicies,
    onPolicyChange,
    onRegistryValueChange
}: Props) => {
    return (
        <Box key={`${policy.policyId}-${index}`} sx={{ display: "flex", gap: 2 }}>
            <Autocomplete
                options={allPolicies}
                getOptionLabel={(option) => option.name}
                value={allPolicies.find(p => p.id === policy.policyId) || null}
                onChange={(event, newValue) => onPolicyChange(index, newValue)}
                renderInput={(params) => <TextField {...params} label="Policy" size="small" />}
                sx={{ flex: 1 }}
            />
            <TextField
                size="small"
                label="Registry Value"
                value={policy.registryValue}
                onChange={(e) => onRegistryValueChange(index, e.target.value)}
                sx={{ flex: 1 }}
            />
        </Box>
    );
};
