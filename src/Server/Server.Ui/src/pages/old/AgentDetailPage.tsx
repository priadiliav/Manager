import React from "react";
import { useParams, Link as RouterLink } from 'react-router-dom';
import { AgentDetailedDto, AgentDto } from "../../types/agent";
import { fetchAgentById } from "../../api/agent";
import Breadcrumbs from '@mui/material/Breadcrumbs';
import Link from '@mui/material/Link';
import Typography from '@mui/material/Typography';
import {
  Table,
  TableBody,
  TableCell,
  TableRow,
  TableContainer,
  Box,
  Stack,
  Card,
  CardContent, Grid
} from "@mui/material";
import { LineChart } from '@mui/x-charts/LineChart';
import { fetchMetrics } from "../../api/metric";

//todo: to be refactored
export const AgentDetailPageOld = () => {
  const { id } = useParams<{ id: string }>();
  const [agent, setAgent] = React.useState<AgentDetailedDto | null>(null);

  const [cpuUsageData, setCpuUsageData] = React.useState<number[]>([]);
  const [memoryUsageData, setMemoryUsageData] = React.useState<number[]>([]);
  const [diskUsageData, setDiskUsageData] = React.useState<number[]>([]);
  const [networkUsageData, setNetworkUsageData] = React.useState<number[]>([]);
  const [error, setError] = React.useState<string | null>(null);

  React.useEffect(() => {
    const loadData = async () => {
      if (!id) return;
      setError(null);
      try {
        const data = await fetchAgentById(id);
        setAgent(data);

        const metricsData = await fetchMetrics(
          data.id,
          new Date(Date.now() - 7 * 24 * 60 * 60 * 1000).toISOString(),
          new Date().toISOString()
        );

        setCpuUsageData(metricsData.map(m => m.cpuUsage));
        setMemoryUsageData(metricsData.map(m => m.memoryUsage));
        setDiskUsageData(metricsData.map(m => m.diskUsage));
        setNetworkUsageData(metricsData.map(m => m.networkUsage));
      } catch (err) {
        console.error("API error:", err);
        setError("Could not load agent data. Please check connection.");
      }
    };

    loadData();
  }, [id]);


  return (
    <>
      <Stack direction="row" justifyContent="space-between" alignItems="center" mb={2}>
        <Breadcrumbs aria-label="breadcrumb">
          <Link component={RouterLink} underline="hover" color="inherit" to="/agents">
            Agents
          </Link>
          <Typography color="text.primary">{agent ? agent.name : 'Loading...'}</Typography>
        </Breadcrumbs>
      </Stack>


      {error && <Typography color="error">{error}</Typography>}
      {!agent && <Typography>Loading...</Typography>}
      {agent && (
        <Grid container spacing={2} height={200}>
          <Grid size={8}>
            <Card sx={{ height: '100%' }}>
              <Typography variant="h6" gutterBottom sx={{ padding: 2 }}>
                Resource Usage
              </Typography>
              <Box >
                <LineChart
                  sx={{ height: 200, width: '100%' }}
                  hideLegend
                  series={[
                    {
                      data: cpuUsageData,
                      area: true,
                      showMark: false
                    },
                    {
                      data: memoryUsageData,
                      showMark: false
                    },
                    {
                      data: diskUsageData,
                      showMark: false
                    },
                  ]}
                />
              </Box>
            </Card>
          </Grid>
          {agent.hardware && (
            <Grid size={4}>
              <Card>
                <CardContent>
                  <Typography variant="h6" gutterBottom>
                    Hardware
                  </Typography>
                  <TableContainer>
                    <Table size="small">
                      <TableBody>
                        <TableRow>
                          <TableCell><strong>CPU</strong></TableCell>
                          <TableCell>
                            {agent.hardware.cpuModel} ({agent.hardware.cpuCores} cores, {agent.hardware.cpuSpeedGHz} GHz)
                          </TableCell>
                        </TableRow>
                        <TableRow>
                          <TableCell><strong>Architecture</strong></TableCell>
                          <TableCell>{agent.hardware.cpuArchitecture}</TableCell>
                        </TableRow>
                        <TableRow>
                          <TableCell><strong>GPU</strong></TableCell>
                          <TableCell>{agent.hardware.gpuModel} ({agent.hardware.gpuMemoryMB} MB)</TableCell>
                        </TableRow>
                        <TableRow>
                          <TableCell><strong>RAM</strong></TableCell>
                          <TableCell>
                            {agent.hardware.ramModel} ({(agent.hardware.totalMemoryMB / 1024).toFixed(1)} GB)
                          </TableCell>
                        </TableRow>
                        <TableRow>
                          <TableCell><strong>Disk</strong></TableCell>
                          <TableCell>
                            {agent.hardware.diskModel} ({(agent.hardware.totalDiskMB / 1024).toFixed(1)} GB)
                          </TableCell>
                        </TableRow>
                      </TableBody>
                    </Table>
                  </TableContainer>
                </CardContent>
              </Card>
            </Grid>)}
        </Grid>
      )}
    </>
  )
}
