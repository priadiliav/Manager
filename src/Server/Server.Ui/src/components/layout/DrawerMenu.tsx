import { Link as RouterLink } from 'react-router-dom';
import Drawer from '@mui/material/Drawer';
import Toolbar from '@mui/material/Toolbar';
import Divider from '@mui/material/Divider';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import SupportAgentIcon from '@mui/icons-material/SupportAgent';
import { Icon } from '@mui/material';
import SpaceDashboardIcon from '@mui/icons-material/SpaceDashboard';
import AdminPanelSettingsIcon from '@mui/icons-material/AdminPanelSettings';//policy
import SettingsRemoteIcon from '@mui/icons-material/SettingsRemote';
import DeveloperBoardIcon from '@mui/icons-material/DeveloperBoard';
import TryIcon from '@mui/icons-material/Try';

interface DrawerMenuProps {
  drawerWidth: number;
}

const firstList = [
  {
    text: 'Dashboard',
    path: '/dashboard',
    icon: <Icon component={SpaceDashboardIcon} />,
    disabled: false
  },
  {
    text: 'Agents',
    path: '/agents',
    icon: <Icon component={SupportAgentIcon} />,
    disabled: false
  },
  {
    text: 'Configurations',
    path: '/configurations',
    icon: <Icon component={SettingsRemoteIcon} />,
    disabled: false
  },
  {
    text: 'Policies',
    path: '/policies',
    icon: <Icon component={AdminPanelSettingsIcon} />,
    disabled: false
  },
  {
    text: 'Processes',
    path: '/processes',
    icon: <Icon component={DeveloperBoardIcon} />,
    disabled: false
  },
  {
    text: 'Chat',
    path: '/chat',
    icon: <Icon component={TryIcon} />,
    disabled: true
  },
];


export default function DrawerMenu({ drawerWidth }: DrawerMenuProps) {
  return (
    <Drawer
      sx={{
        width: drawerWidth,
        flexShrink: 0,
        '& .MuiDrawer-paper': {
          width: drawerWidth,
          boxSizing: 'border-box',
        },
      }}
      variant="permanent"
      anchor="left"
    >
      <Toolbar />
      <Divider />
      <List>
        {firstList.map(({ text, path, icon, disabled }, index) => (
          <ListItem key={index} disablePadding>
            <ListItemButton component={RouterLink} to={path} disabled={disabled}>
              <ListItemIcon>
                {icon}
              </ListItemIcon>
              <ListItemText primary={text} />
            </ListItemButton>
          </ListItem>
        ))}
      </List>
    </Drawer>
  );
}
