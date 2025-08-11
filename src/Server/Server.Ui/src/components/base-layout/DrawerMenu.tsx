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
import PodcastsIcon from '@mui/icons-material/Podcasts';
interface DrawerMenuProps {
  drawerWidth: number;
}

const firstList = [
  {
    text: 'Dashboard',
    path: '/dashboard',
    icon: <Icon component={SpaceDashboardIcon} />
  },
  {
    text: 'Agents',
    path: '/agents',
    icon: <Icon component={SupportAgentIcon} />
  },
  {
    text: 'Cluster',
    path: '/cluster',
    icon: <Icon component={PodcastsIcon} />
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
        {firstList.map(({ text, path, icon }, index) => (
          <ListItem key={index} disablePadding>
            <ListItemButton component={RouterLink} to={path}>
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
