import { useState } from "react";
import { HardwareDto } from "../../types/hardware";
import { Box, Card, CardContent, Collapse, IconButton, Table, TableBody, TableCell, TableContainer, TableRow, Typography } from "@mui/material";
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';

interface Props {
    initialData: HardwareDto;
}

export const HardwareInformation = ({ initialData }: Props) => {
    const [isVisible, setIsVisible] = useState<boolean>(true);
    return (
        <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
            <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between" }}>
                <Typography variant="h6">
                    Hardware Information
                </Typography>
                <IconButton size="small" onClick={() => setIsVisible(!isVisible)}>
                    <ExpandMoreIcon
                        sx={{
                            transform: isVisible ? 'rotate(180deg)' : 'rotate(0deg)',
                            transition: 'transform 0.3s'
                        }}
                    />
                </IconButton>
            </Box>
            <Collapse in={isVisible} timeout="auto" unmountOnExit>
                <Box sx={{ display: "flex", flexDirection: "column", gap: 1 }}>
                    <Card>
                        <CardContent>
                            <TableContainer>
                                <Table size="small">
                                    <TableBody>
                                        <TableRow>
                                            <TableCell><strong>CPU</strong></TableCell>
                                            <TableCell>
                                                ({initialData.cpuCores} cores, {initialData.cpuSpeedGHz} GHz)
                                            </TableCell>
                                        </TableRow>
                                        <TableRow>
                                            <TableCell><strong>Architecture</strong></TableCell>
                                            <TableCell>{initialData.cpuArchitecture}</TableCell>
                                        </TableRow>
                                        <TableRow>
                                            <TableCell><strong>GPU</strong></TableCell>
                                            <TableCell>{initialData.gpuModel} ({initialData.gpuMemoryMB} MB)</TableCell>
                                        </TableRow>
                                        <TableRow>
                                            <TableCell><strong>RAM</strong></TableCell>
                                            <TableCell>
                                                {initialData.ramModel} ({(initialData.totalMemoryMB / 1024).toFixed(1)} GB)
                                            </TableCell>
                                        </TableRow>
                                        <TableRow>
                                            <TableCell><strong>Disk</strong></TableCell>
                                            <TableCell>
                                                {initialData.diskModel} ({(initialData.totalDiskMB / 1024).toFixed(1)} GB)
                                            </TableCell>
                                        </TableRow>
                                    </TableBody>
                                </Table>
                            </TableContainer>
                        </CardContent>
                    </Card>
                </Box>
            </Collapse >
        </Box >
    );
}