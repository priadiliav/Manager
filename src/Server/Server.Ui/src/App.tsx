import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { DashboardPage } from './pages/dashboard/DashboardPage';
import { ClusterPage } from './pages/cluster/ClusterPage';
import { AgentsPage } from './pages/agent/AgentsPage';
import { ConfigurationsPage } from './pages/configuration/ConfigurationsPage';
import { PoliciesPage } from './pages/policies/PoliciesPage';
import Layout from './components/layout/Layout';
import { ProcessesPage } from './pages/process/ProcessesPage';
import { AgentPage } from './pages/agent/AgentPage';
import { PolicyPage } from './pages/policies/PolicyPage';
import { ProcessPage } from './pages/process/ProcessPage';
import { ConfigurationPage } from './pages/configuration/ConfigurationPage';


const demoTheme = createTheme({
  cssVariables: {
    colorSchemeSelector: 'data-toolpad-color-scheme',
  },
  typography: {
    // htmlFontSize: 18.4,
  },
  colorSchemes: { light: true, dark: true },
  breakpoints: {
    values: {
      xs: 0,
      sm: 600,
      md: 600,
      lg: 1200,
      xl: 1536,
    },
  },
});

export default function App() {
  // @ts-ignore
  return (
    <ThemeProvider theme={demoTheme}>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Layout />}>
            <Route index element={<Navigate to="/dashboard" replace />} />
            <Route path="dashboard" element={<DashboardPage />} />

            <Route path="agents" element={<AgentsPage />} />
            <Route path="agents/:id" element={<AgentPage />} />

            <Route path="configurations" element={<ConfigurationsPage />} />
            <Route path="configurations/:id" element={<ConfigurationPage />} />

            <Route path="policies" element={<PoliciesPage />} />
            <Route path="policies/:id" element={<PolicyPage />} />

            <Route path="processes" element={<ProcessesPage />} />
            <Route path="processes/:id" element={<ProcessPage />} />

            {/* <Route path="/agents/:id" element={<AgentDetailPage />} />

            <Route path="configurations/:id" element={<ConfigurationDetailPage />} /> */}

            <Route path="cluster" element={<ClusterPage />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </ThemeProvider>
  );
}
