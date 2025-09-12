import { Box, FormControlLabel, Grid, Switch, TextField, Typography } from "@mui/material";
import { CpuUsageLineChart } from "./CpuUsageLineChart";
import { MemoryUsageLineChart } from "./MemoryUsageLineChart";
import { DiskUsageLineChart } from "./DiskUsageLineChart";
import { NetworkUsageLineChart } from "./NetworkUsageLineChart";
import { useEffect, useState } from "react";
import { fetchMetrics } from "../../api/metric";
import FetchContentWrapper from "../wrappers/FetchContentWrapper";
import { CollapsibleSection } from "../wrappers/CollapsibleSection";

export const AgentCharts = ({ agentId }: { agentId: string }) => {
    const [cpuUsageLineChartData, setCpuUsageLineChartData] = useState<number[]>([]);
    const [memoryUsageLineChartData, setMemoryUsageLineChartData] = useState<number[]>([]);
    const [diskUsageLineChartData, setDiskUsageLineChartData] = useState<number[]>([]);
    const [networkUsageLineChartData, setNetworkUsageLineChartData] = useState<number[]>([]);
    const [labels, setLabels] = useState<string[]>([]);

    const [isRealTimeMonitoring, setIsRealTimeMonitoring] = useState(false);
    const [from, setFrom] = useState<Date>(new Date(new Date().getTime() - 12 * 60 * 60 * 1000)); // 12 hours ago
    const [to, setTo] = useState<Date>(() => {
        const defaultTo = new Date();
        defaultTo.setHours(defaultTo.getHours() + 12);
        return defaultTo;
    });
    const [limit, setLimit] = useState<number>(50);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        fetchMetricsData(from, to);
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [from, to, limit]);

    //#region API calls
    const fetchMetricsData = async (from: Date, to: Date) => {
        try {
            const fromIso = from.toISOString();
            const toIso = to.toISOString();

            setLoading(true);
            const data = await fetchMetrics(agentId, fromIso, toIso, limit);

            const cpuData = data.map(d => d.cpuUsage);
            const memoryData = data.map(d => d.memoryUsage);
            const diskData = data.map(d => d.diskUsage);
            const networkData = data.map(d => d.networkUsage);

            setCpuUsageLineChartData(cpuData);
            setMemoryUsageLineChartData(memoryData);
            setDiskUsageLineChartData(diskData);
            setNetworkUsageLineChartData(networkData);
            setLabels(data.map(d => new Date(d.timestamp).toLocaleString()));
        } catch (error) {
            console.error("Error fetching metrics:", error);
            setError("Failed to load metrics data");
        } finally {
            setLoading(false);
        }
    }
    //#endregion

    return (
        <FetchContentWrapper loading={loading} error={error}>
            <CollapsibleSection title="Resource Usage Charts">
                <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between" }}>
                    <FormControlLabel
                        control={
                            <Switch
                                name="isActive"
                                color="primary"
                                checked={isRealTimeMonitoring}
                                onChange={() => setIsRealTimeMonitoring(!isRealTimeMonitoring)}
                            />
                        }
                        label={<Typography>Real time monitoring</Typography>}
                        labelPlacement="end"
                    />
                    <Box sx={{ display: "flex", gap: 2 }}>
                        <TextField
                            size="small"
                            label="From"
                            type="datetime-local"
                            disabled={isRealTimeMonitoring}
                            value={from ? from.toISOString().slice(0, 16) : ""}
                            onChange={(e) => setFrom(e.target.value ? new Date(e.target.value) : new Date())}
                            InputLabelProps={{
                                shrink: true,
                            }}
                        />
                        <TextField
                            size="small"
                            label="To"
                            type="datetime-local"
                            disabled={isRealTimeMonitoring}
                            value={to ? to.toISOString().slice(0, 16) : ""}
                            onChange={(e) => setTo(e.target.value ? new Date(e.target.value) : new Date())}
                            InputLabelProps={{
                                shrink: true,
                            }}
                        />
                        <TextField
                            size="small"
                            label="Data Points Limit"
                            type="number"
                            disabled={isRealTimeMonitoring}
                            value={limit}
                            onChange={(e) => setLimit(Number(e.target.value))}
                            InputLabelProps={{
                                shrink: true,
                            }}
                            inputProps={{ min: 1, max: 500 }}
                        />
                    </Box>
                </Box>
                <Box sx={{ display: "flex", flexDirection: "column" }}>
                    <Grid sx={{ md: 12 }} >
                        <CpuUsageLineChart data={cpuUsageLineChartData} labels={labels} />
                    </Grid>
                    <Grid sx={{ md: 12 }}>
                        <MemoryUsageLineChart data={memoryUsageLineChartData} labels={labels} />
                    </Grid>
                    <Grid sx={{ md: 12 }}>
                        <DiskUsageLineChart data={diskUsageLineChartData} labels={labels} />
                    </Grid>
                    <Grid sx={{ md: 12 }}>
                        <NetworkUsageLineChart data={networkUsageLineChartData} labels={labels} />
                    </Grid>
                </Box>
            </CollapsibleSection>
        </FetchContentWrapper>
    );
};
