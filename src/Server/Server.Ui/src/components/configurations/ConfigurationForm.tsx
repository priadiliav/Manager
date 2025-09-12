import { Box, Button, Grid, TextField } from "@mui/material";
import { useEffect, useState } from "react";
import { ConfigurationCreateRequest, ConfigurationDetailedDto, ConfigurationModifyRequest } from "../../types/configuration";
import { PolicyDto } from "../../types/policy";
import { ProcessDto, ProcessState } from "../../types/process";
import { PolicyInConfigurationForm } from "./PolicyInConfigurationForm";
import { ProcessInConfigurationForm } from "./ProcessInConfigurationForm";
import { CollapsibleSection } from "../wrappers/CollapsibleSection";

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
                    <CollapsibleSection title="General Information">
                        <Box sx={{ display: "flex", flexDirection: "column", gap: 2, mt: 2 }}>
                            <TextField
                                label="Name"
                                name="name"
                                value={formData.name}
                                onChange={handleChange}
                                fullWidth
                                size="small"
                            />
                        </Box>
                    </CollapsibleSection>
                </Grid>

                <Grid size={{ xs: 12, md: 12 }}>
                    <CollapsibleSection title="Policies">
                        <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
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
                                onClick={() =>
                                    setFormData({
                                        ...formData,
                                        policies: [...formData.policies, { policyId: "", registryValue: "" }]
                                    })
                                }
                            >
                                Add Policy
                            </Button>
                        </Box>
                    </CollapsibleSection>
                </Grid>

                <Grid size={{ xs: 12, md: 12 }}>
                    <CollapsibleSection title="Processes">
                        <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
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
                                onClick={() =>
                                    setFormData({
                                        ...formData,
                                        processes: [...formData.processes, { processId: "", processState: ProcessState.Banned }]
                                    })
                                }
                            >
                                Add Process
                            </Button>
                        </Box>
                    </CollapsibleSection>
                </Grid>
            </Grid>
        </Box>
    );
};
