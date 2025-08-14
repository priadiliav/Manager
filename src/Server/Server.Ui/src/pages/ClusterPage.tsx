import api from '../api/axios';
import {useEffect, useState} from "react";

export interface Deployment {
  name: string;
  namespace: string;
  image: string;
  status: string;
  pods: Pod[];
}

export interface Pod {
  name: string;
  namespace: string;
  status: string;
}

export const ClusterPage = () => {
  const [deployments, setDeployments] = useState<Deployment[]>([]);
  const fetchClusterData = async () => {
    try {
      const response = await api.get('/cluster/deployments');
      if (response.status === 200) {
        setDeployments(response.data);
      } else {
        console.error('Failed to fetch cluster data:', response.statusText);
      }
    } catch (error) {
      console.error('Error fetching cluster data:', error);
    }
  };

  useEffect(() => {
    fetchClusterData();
  }, []);

  return (
    <>
      Cluster info
    </>
  );
}
