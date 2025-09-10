import { Autocomplete, Box, TextField } from "@mui/material";
import { ProcessDto, ProcessInConfigurationDto, ProcessState } from "../../types/process";

interface Props {
    process: ProcessInConfigurationDto;
    index: number;
    allProcesses: ProcessDto[];
    valueTypeOptions: ProcessState[];
    onProcessChange: (index: number, newProcess: ProcessDto | null) => void;
    onProcessStateChange: (index: number, state: ProcessState) => void;
}

export const ProcessInConfigurationForm = ({
    process,
    index,
    allProcesses,
    valueTypeOptions,
    onProcessChange,
    onProcessStateChange
}: Props) => {
    return (
        <Box key={`${process.processId}-${index}`} sx={{ display: "flex", justifyContent: "space-between", gap: 2 }}>
            <Autocomplete
                options={allProcesses}
                getOptionLabel={(option) => option.name}
                value={allProcesses.find(p => p.id === process.processId) || null}
                onChange={(event, newValue) => onProcessChange(index, newValue)}
                renderInput={(params) => <TextField {...params} label="Process" size="small" />}
                sx={{ flex: 1 }}
            />
            <Autocomplete
                options={valueTypeOptions}
                getOptionLabel={(option) => ProcessState[option]}
                value={valueTypeOptions.find(v => v === process.processState) || null}
                onChange={(event, newValue) => onProcessStateChange(index, newValue || ProcessState.Banned)}
                renderInput={(params) => <TextField {...params} label="Process State" size="small" />}
            />
        </Box>
    );
};
