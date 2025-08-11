import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';
import WifiIcon from '@mui/icons-material/Wifi';

interface AppBarHeaderProps {
  drawerWidth: number;
}

export default function AppBarHeader(props: AppBarHeaderProps) {
  // @ts-ignore
  const backendUrl = window._env_.BACKEND_URL

  if (!backendUrl) {
    console.error('BACKEND_URL is not defined');
    return null;
  }

  return (
    <AppBar
      position="fixed"
      sx={{
        width: `calc(100% - ${props.drawerWidth}px)`,
        ml: `${props.drawerWidth}px`,
      }}
    >
      <Toolbar sx={{ display: 'flex', justifyContent: 'space-between' }}>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <WifiIcon fontSize="small" />
          <Typography variant="body2" sx={{ fontSize: '0.875rem' }}>
            Connected to: <strong>{backendUrl}</strong>
          </Typography>
        </Box>
      </Toolbar>
    </AppBar>
  );
}
