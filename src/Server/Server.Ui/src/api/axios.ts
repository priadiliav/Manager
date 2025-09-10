import axios from 'axios';
import { env } from '../env';

const api = axios.create({
  baseURL: env.BACKEND_URL,
  headers: {
    'Content-Type': 'application/json'
  }
});

api.interceptors.response.use(
  response => response,
  error => {
    if (error.message === "Network Error") {
      console.error("Backend unreachable:", error);
    }
    return Promise.reject(error);
  }
);

export default api;
