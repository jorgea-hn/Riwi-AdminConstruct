import api from './api';

interface CustomerProfile {
    id: string;
    name: string;
    document: string;
    email: string;
    phone?: string;
    userId?: string;
}

export const profileService = {
    async getMyProfile(): Promise<CustomerProfile> {
        const response = await api.get<CustomerProfile>('/customers/my-profile');
        return response.data;
    },

    async updateMyProfile(data: { name: string; document: string; phone?: string; email: string }): Promise<CustomerProfile> {
        const response = await api.put<CustomerProfile>('/customers/my-profile', data);
        return response.data;
    },

    // Validation methods removed as profile completion is now handled during registration

};
