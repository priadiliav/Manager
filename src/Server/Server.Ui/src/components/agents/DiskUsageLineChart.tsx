import { Box, Collapse, IconButton, Typography } from "@mui/material";
import { useState } from "react";
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import { CustomLineChart } from "../charts/CustomLineChart";

interface Props {
    data: number[];
    labels: string[];
}

export const DiskUsageLineChart = ({ data, labels }: Props) => {
    const [isVisible, saveIsVisible] = useState(false);

    return (
        <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }} >
            <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between" }}>
                <Typography variant="h6">Disk Usage</Typography>
                <IconButton size="small" onClick={() => saveIsVisible(!isVisible)}>
                    <ExpandMoreIcon
                        sx={{
                            transform: isVisible ? 'rotate(180deg)' : 'rotate(0deg)',
                            transition: 'transform 0.3s'
                        }}
                    />
                </IconButton>
            </Box>
            <Collapse in={isVisible} timeout="auto" unmountOnExit>
                <Box sx={{ height: 300 }}>
                    <CustomLineChart data={data} labels={labels} />
                </Box>
            </Collapse>
        </Box>
    );
} 