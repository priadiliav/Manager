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
  return (
    <>
      cluster page
    </>
  );
}
