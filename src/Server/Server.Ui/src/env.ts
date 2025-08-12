export interface EnvConfig {
  BACKEND_URL: string;
}

declare global {
  interface Window {
    _env_?: EnvConfig;
  }
}

export const env: EnvConfig = {
  BACKEND_URL: window._env_?.BACKEND_URL || ''
};
