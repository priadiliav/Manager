import { Box, Button, Grid, TextField, Typography } from "@mui/material";
import { useEffect, useState } from "react";
import { ConfigurationCreateRequest, ConfigurationDetailedDto, ConfigurationModifyRequest } from "../../types/configuration";
import { PolicyDto } from "../../types/policy";
import { ProcessDto, ProcessState } from "../../types/process";
import { PolicyInConfigurationForm } from "./PolicyInConfigurationForm";
import { ProcessInConfigurationForm } from "./ProcessInConfigurationForm";

interface Props {
    initialData: ConfigurationDetailedDto;
    initialPolicies: PolicyDto[];
    initialProcesses: ProcessDto[];
    mode: "create" | "edit";
    onChange: (data: ConfigurationCreateRequest | ConfigurationModifyRequest) => void;
}

const valueTypeOptions = Object.values(ProcessState).filter(
    (v) => typeof v === "number"
) as ProcessState[];

export const ConfigurationForm = ({ initialData, initialPolicies, initialProcesses, mode, onChange }: Props) => {
    const [formData, setFormData] = useState<ConfigurationCreateRequest | ConfigurationModifyRequest>({
        name: initialData.name,
        policies: initialData.policies,
        processes: initialData.processes
    });

    useEffect(() => {
        onChange(formData);
    }, [formData, onChange]);

    //#region Handlers
    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handlePolicyChange = (index: number, newPolicy: PolicyDto | null) => {
        const newPolicies = [...formData.policies];
        newPolicies[index].policyId = newPolicy?.id || "";
        setFormData({ ...formData, policies: newPolicies });
    };

    const handleRegistryValueChange = (index: number, value: string) => {
        const newPolicies = [...formData.policies];
        newPolicies[index].registryValue = value;
        setFormData({ ...formData, policies: newPolicies });
    };

    const handleProcessChange = (index: number, newProcess: ProcessDto | null) => {
        const newProcesses = [...formData.processes];
        newProcesses[index].processId = newProcess?.id || "";
        setFormData({ ...formData, processes: newProcesses });
    };

    const handleProcessStateChange = (index: number, state: ProcessState) => {
        const newProcesses = [...formData.processes];
        newProcesses[index].processState = state;
        setFormData({ ...formData, processes: newProcesses });
    };
    //#endregion

    return (
        <Box component="form" sx={{ display: "flex", flexDirection: "column", gap: 2 }}>

            <Grid container spacing={2}>
                <Grid size={{ xs: 12, md: 12 }}>
                    <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }} >
                        <Typography variant="h6">General Information</Typography>
                        <TextField
                            size="small"
                            name="name"
                            label="Configuration Name"
                            value={formData.name}
                            onChange={handleChange}
                            required
                        />
                    </Box>
                </Grid>
                <Grid size={{ xs: 12, md: 6 }}>
                    <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }} >
                        <Typography variant="h6">Policies</Typography>
                        {formData.policies.map((policy, index) => (
                            <PolicyInConfigurationForm
                                key={`${policy.policyId}-${index}`}
                                policy={policy}
                                index={index}
                                allPolicies={initialPolicies}
                                onPolicyChange={handlePolicyChange}
                                onRegistryValueChange={handleRegistryValueChange}
                            />
                        ))}
                        <Button
                            variant="outlined"
                            size="small"
                            onClick={() => setFormData({
                                ...formData,
                                policies: [...formData.policies, { policyId: "", registryValue: "" }]
                            })}
                        >
                            Add Policy
                        </Button>
                    </Box>
                </Grid>

                <Grid size={{ xs: 12, md: 6 }}>
                    <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
                        <Typography variant="h6">Processes</Typography>
                        {formData.processes.map((process, index) => (
                            <ProcessInConfigurationForm
                                key={`${process.processId}-${index}`}
                                process={process}
                                index={index}
                                allProcesses={initialProcesses}
                                valueTypeOptions={valueTypeOptions}
                                onProcessChange={handleProcessChange}
                                onProcessStateChange={handleProcessStateChange}
                            />
                        ))}
                        <Button
                            variant="outlined"
                            size="small"
                            onClick={() => setFormData({
                                ...formData,
                                processes: [...formData.processes, { processId: "", processState: ProcessState.Banned }]
                            })}
                        >
                            Add Process
                        </Button>
                    </Box>
                </Grid>
            </Grid>
        </Box>
    );
};
