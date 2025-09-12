import { Box } from "@mui/material";
import { CustomLineChart } from "../charts/CustomLineChart";
import { CollapsibleSection } from "../wrappers/CollapsibleSection";

interface Props {
    data: number[];
    labels: string[];
}

export const DiskUsageLineChart = ({ data, labels }: Props) => {
    return (
        <CollapsibleSection title="Disk Usage (%)" noCard>
            <Box sx={{ height: 300 }}>
                <CustomLineChart data={data} labels={labels} />
            </Box>
        </CollapsibleSection>
    );
};
