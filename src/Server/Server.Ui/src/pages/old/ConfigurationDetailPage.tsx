import { Alert, CircularProgress, Stack, Button, MenuItem, Select, InputLabel, FormControl } from "@mui/material";
import Breadcrumbs from "@mui/material/Breadcrumbs";
import Link from "@mui/material/Link";
import { Link as RouterLink, useParams } from "react-router-dom";
import React from "react";
import { ConfigurationDetailedDto } from "../../types/configuration";
import { fetchConfigurationById } from "../../api/configuration";
import { Box, Typography, Card, CardContent, Grid, TextField, IconButton } from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete';
import { ProcessInConfigurationDto, ProcessState } from "../../types/process";

interface Policy {
  type: string;
  value: string;
}

export const ConfigurationDetailPageOld = () => {
  const { id } = useParams<{ id: string }>();
  const [loading, setLoading] = React.useState(true);
  const [error, setError] = React.useState<string | null>(null);
  const [configuration, setConfiguration] = React.useState<ConfigurationDetailedDto | null>(null);
  const [policies, setPolicies] = React.useState<Policy[]>([{ type: '', value: '' }]);
  const [processes, setProcesses] = React.useState<ProcessInConfigurationDto[]>([
    {
      processId: '',
      processState: ProcessState.Active
    }
  ]);

  React.useEffect(() => {
    const loadConfiguration = async () => {
      if (!id) return;
      setError(null);
      try {
        const data = await fetchConfigurationById(id);
        setConfiguration(data);
      } catch (err) {
        console.error("API error:", err);
        setError("Could not load configuration data. Please check connection.");
      } finally {
        setLoading(false);
      }
    };
    loadConfiguration();
  }, [id]);

  const handleAddPolicy = () => {
    setPolicies([...policies, { type: '', value: '' }]);
  };

  const handleRemovePolicy = (index: number) => {
    setPolicies(policies.filter((_, i) => i !== index));
  };

  const handlePolicyChange = (index: number, field: keyof Policy, value: string) => {
    const newPolicies = [...policies];
    newPolicies[index] = { ...newPolicies[index], [field]: value };
    setPolicies(newPolicies);
  };

  const policyTypes = [
    { value: 'maxConnections', label: 'Max Connections' },
    { value: 'timeout', label: 'Timeout (ms)' },
    { value: 'rateLimit', label: 'Rate Limit' },
    { value: 'cacheSize', label: 'Cache Size' },
  ];

  return (
    <>
      <Stack direction="row" justifyContent="space-between" alignItems="center" mb={2}>
        <Breadcrumbs aria-label="breadcrumb">
          <Link component={RouterLink} underline="hover" color="inherit" to="/configurations">
            Configurations
          </Link>
          <Typography color="text.primary">{configuration ? configuration.name : 'Loading...'}</Typography>
        </Breadcrumbs>

        <Button variant="contained">
          Save Changes
        </Button>
      </Stack>

      {loading && (
        <Box display="flex" justifyContent="center" alignItems="center" height={200}>
          <CircularProgress />
        </Box>
      )}

      {error && (
        <Alert severity="error">{error}</Alert>
      )}

      {!loading && !error && configuration && (
        <Grid container spacing={2}>
          <Grid size={6}>
            <Card sx={{ mb: 3 }}>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Policies
                </Typography>
                {policies.map((policy, index) => (
                  <Grid container spacing={2} key={index} alignItems="center" sx={{ mb: 2 }}>
                    <Grid size={5}>
                      <FormControl fullWidth size="small">
                        <InputLabel>Policy Type</InputLabel>
                        <Select
                          value={policy.type}
                          label="Policy Type"
                          onChange={(e) => handlePolicyChange(index, 'type', e.target.value)}
                        >
                          {policyTypes.map((type) => (
                            <MenuItem key={type.value} value={type.value}>
                              {type.label}
                            </MenuItem>
                          ))}
                        </Select>
                      </FormControl>
                    </Grid>
                    <Grid size={5}>
                      <TextField
                        fullWidth
                        label="Value"
                        type="text"
                        size="small"
                        value={policy.value}
                        onChange={(e) => handlePolicyChange(index, 'value', e.target.value)}
                      />
                    </Grid>
                    <Grid size={2}>
                      <IconButton onClick={() => handleRemovePolicy(index)} disabled={policies.length === 1} size={"small"}>
                        <DeleteIcon />
                      </IconButton>
                    </Grid>
                  </Grid>
                ))}
                <Button variant="contained" onClick={handleAddPolicy} sx={{ mt: 2 }}>
                  Add Policy
                </Button>
              </CardContent>
            </Card>
          </Grid>
          <Grid size={6}>
            <Card>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Processes
                </Typography>
                {processes.map((process, index) => (
                  <Grid container spacing={2} key={index} alignItems="center" sx={{ mb: 2 }}>
                    <Grid size={5}>
                      <FormControl fullWidth size="small">
                        <InputLabel>Process</InputLabel>
                        <Select
                          label="Policy Type"
                        >
                          {policyTypes.map((type) => (
                            <MenuItem key={type.value} value={type.value}>
                              {type.label}
                            </MenuItem>
                          ))}
                        </Select>
                      </FormControl>
                    </Grid>

                    <Grid size={5}>
                      <FormControl fullWidth size="small">
                        <InputLabel>Process</InputLabel>
                        <Select
                          label="Policy Type"
                        >
                          {policyTypes.map((type) => (
                            <MenuItem key={type.value} value={type.value}>
                              {type.label}
                            </MenuItem>
                          ))}
                        </Select>
                      </FormControl>
                    </Grid>
                  </Grid>
                ))}
              </CardContent>
            </Card>
          </Grid>
        </Grid>
      )}
    </>
  );
};
