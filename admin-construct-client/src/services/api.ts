import axios from 'axios';

const API_URL = 'http://localhost:5228/api';

const api = axios.create({
    baseURL: API_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Interceptor para agregar el token JWT a las peticiones
api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('token');
        console.log('[API] Request to:', config.url);
        console.log('[API] Token exists:', !!token);
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
            console.log('[API] Token added to request');
        } else {
            console.warn('[API] No token found in localStorage');
        }
        return config;
    },
    (error) => {
        console.error('[API] Request error:', error);
        return Promise.reject(error);
    }
);

// Interceptor para manejar errores de autenticación
api.interceptors.response.use(
    (response) => {
        console.log('[API] Response from:', response.config.url, 'Status:', response.status);
        return response;
    },
    (error) => {
        console.error('[API] Response error:', error.response?.status, error.response?.data);

        // Solo redirigir al login si es un error de autenticación en endpoints de auth
        // NO eliminar el token automáticamente para permitir debugging
        if (error.response?.status === 401) {
            console.error('[API] 401 Unauthorized - Check token validity');
            console.error('[API] Current token:', localStorage.getItem('token')?.substring(0, 50) + '...');
        }

        return Promise.reject(error);
    }
);

export default api;
