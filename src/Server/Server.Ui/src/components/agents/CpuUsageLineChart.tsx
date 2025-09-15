import { Box } from "@mui/material";
import { CustomLineChart } from "../charts/CustomLineChart";
import { CollapsibleSection } from "../wrappers/CollapsibleSection";

interface Props {
    data: number[];
    labels: string[];
}

export const CpuUsageLineChart = ({ data, labels }: Props) => {
    return (
        <CollapsibleSection title="CPU Usage (%)" noCard defaultOpen={false}>
            <Box sx={{ height: 300 }}>
                <CustomLineChart data={data} labels={labels} />
            </Box>
        </CollapsibleSection>
    );
};
