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
      const response = await api.get('/api/cluster/deployments');
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
      <h1>Cluster Deployments</h1>
      <table>
        <thead>
          <tr>
            <th>Name</th>
            <th>Namespace</th>
            <th>Image</th>
            <th>Status</th>
            <th>Pods</th>
          </tr>
        </thead>
        <tbody>
          {deployments.map((deployment) => (
            <tr key={`${deployment.namespace}/${deployment.name}`}>
              <td>{deployment.name}</td>
              <td>{deployment.namespace}</td>
              <td>{deployment.image}</td>
              <td>{deployment.status}</td>
              <td>
                {deployment.pods.map((pod) => (
                  <div key={`${pod.namespace}/${pod.name}`}>{pod.name} ({pod.status})</div>
                ))}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </>
  );
}
