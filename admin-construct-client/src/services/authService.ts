import api from './api';
import { jwtDecode } from 'jwt-decode';

export interface LoginDto {
    email: string;
    password: string;
}

export interface RegisterDto {
    email: string;
    password: string;
    name: string;
    document: string;
    phone?: string;
}

export interface AuthResponse {
    token: string;
    expiration: string;
}

export interface DecodedToken {
    sub: string;
    role: string;
    exp: number;
}

export const authService = {
    async login(credentials: LoginDto): Promise<AuthResponse> {
        const response = await api.post<AuthResponse>('/auth/login', credentials);
        if (response.data.token) {
            localStorage.setItem('token', response.data.token);
        }
        return response.data;
    },

    async register(data: RegisterDto): Promise<{ message: string, customerId: string }> {
        const response = await api.post<{ message: string, customerId: string }>('/auth/register', data);
        return response.data;
    },

    logout() {
        localStorage.removeItem('token');
        window.location.href = '/login';
    },

    getToken(): string | null {
        return localStorage.getItem('token');
    },

    isAuthenticated(): boolean {
        const token = this.getToken();
        if (!token) return false;

        try {
            const decoded: DecodedToken = jwtDecode(token);
            return decoded.exp * 1000 > Date.now();
        } catch {
            return false;
        }
    },

    getUserEmail(): string | null {
        const token = this.getToken();
        if (!token) return null;

        try {
            const decoded: DecodedToken = jwtDecode(token);
            return decoded.sub;
        } catch {
            return null;
        }
    },
};
