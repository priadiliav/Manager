import { Box } from "@mui/material";
import { CustomLineChart } from "../charts/CustomLineChart";
import { CollapsibleSection } from "../wrappers/CollapsibleSection";

interface Props {
    data: number[];
    labels: string[];
}

export const MemoryUsageLineChart = ({ data, labels }: Props) => {
    return (
        <CollapsibleSection title="Memory Usage (%)" noCard>
            <Box sx={{ height: 300 }}>
                <CustomLineChart data={data} labels={labels} />
            </Box>
        </CollapsibleSection>
    );
};
