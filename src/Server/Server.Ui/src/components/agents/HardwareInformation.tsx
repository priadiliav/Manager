import { CollapsibleSection } from "../wrappers/CollapsibleSection";
import { HardwareDto } from "../../types/hardware";
import { KeyValueTable } from "../table/KeyValueTable";

interface Props {
    initialData: HardwareDto;
}

export const HardwareInformation = ({ initialData }: Props) => {
    const rows = [
        { key: "CPU", value: `${initialData.cpuModel} (${initialData.cpuCores} cores, ${initialData.cpuSpeedGHz} GHz)` },
        { key: "Architecture", value: initialData.cpuArchitecture },
        { key: "GPU", value: `${initialData.gpuModel} (${initialData.gpuMemoryMB} MB)` },
        { key: "RAM", value: `${initialData.ramModel} (${(initialData.totalMemoryMB / 1024).toFixed(1)} GB)` },
        { key: "Disk", value: `${initialData.diskModel} (${(initialData.totalDiskMB / 1024).toFixed(1)} GB)` },
    ];

    return (
        <CollapsibleSection title="Hardware Information" >
            <KeyValueTable rows={rows} />
        </CollapsibleSection>
    );
};
