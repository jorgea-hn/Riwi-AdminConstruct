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
    [key: string]: any;
}

export const authService = {
    async login(credentials: LoginDto): Promise<AuthResponse> {
        console.log('[AuthService] Attempting login for:', credentials.email);
        const response = await api.post<AuthResponse>('/auth/login', credentials);
        if (response.data.token) {
            console.log('[AuthService] Token received, storing in localStorage');
            localStorage.setItem('token', response.data.token);
            console.log('[AuthService] Token stored successfully');

            // Verify token was stored
            const storedToken = localStorage.getItem('token');
            console.log('[AuthService] Token verification:', !!storedToken);

            // Decode and log token info
            try {
                const decoded: DecodedToken = jwtDecode(response.data.token);
                console.log('[AuthService] Token decoded:', {
                    sub: decoded.sub,
                    exp: new Date(decoded.exp * 1000).toISOString(),
                    claims: Object.keys(decoded)
                });
            } catch (e) {
                console.error('[AuthService] Failed to decode token:', e);
            }
        }
        return response.data;
    },

    async register(data: RegisterDto): Promise<{ message: string, customerId: string }> {
        console.log('[AuthService] Attempting registration for:', data.email);
        const response = await api.post<{ message: string, customerId: string }>('/auth/register', data);
        console.log('[AuthService] Registration successful');
        return response.data;
    },

    logout() {
        console.log('[AuthService] Logging out, removing token');
        localStorage.removeItem('token');
        window.location.href = '/login';
    },

    getToken(): string | null {
        const token = localStorage.getItem('token');
        console.log('[AuthService] getToken called, token exists:', !!token);
        return token;
    },

    isAuthenticated(): boolean {
        const token = this.getToken();
        if (!token) {
            console.log('[AuthService] isAuthenticated: No token found');
            return false;
        }

        try {
            const decoded: DecodedToken = jwtDecode(token);
            const isValid = decoded.exp * 1000 > Date.now();
            console.log('[AuthService] isAuthenticated:', isValid, 'Expires:', new Date(decoded.exp * 1000).toISOString());
            return isValid;
        } catch (e) {
            console.error('[AuthService] isAuthenticated: Token decode failed', e);
            return false;
        }
    },

    getUserEmail(): string | null {
        const token = this.getToken();
        if (!token) return null;

        try {
            const decoded: DecodedToken = jwtDecode(token);
            // Intentar obtener el email de diferentes claims
            return decoded.email || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || decoded.sub || null;
        } catch {
            return null;
        }
    },
};
