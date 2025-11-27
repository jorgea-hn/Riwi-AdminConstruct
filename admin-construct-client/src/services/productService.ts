import api from './api';

export interface ProductDto {
    id: string;
    name: string;
    price: number;
    stockQuantity: number;
    description?: string;
    imageUrl?: string;
}

export interface MachineryDto {
    id: number;
    name: string;
    description?: string;
    stock: number;
    price: number;
    isActive: boolean;
    imageUrl?: string;
}

export interface PaginatedResult<T> {
    items: T[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
}

export const productService = {
    async getProducts(page: number = 1, pageSize: number = 10): Promise<PaginatedResult<ProductDto>> {
        const response = await api.get<PaginatedResult<ProductDto>>(`/products?page=${page}&pageSize=${pageSize}`);
        return response.data;
    },

    async getMachinery(page: number = 1, pageSize: number = 10): Promise<PaginatedResult<MachineryDto>> {
        const response = await api.get<PaginatedResult<MachineryDto>>(`/machinery?page=${page}&pageSize=${pageSize}`);
        return response.data;
    },

    async getProductById(id: string): Promise<ProductDto> {
        const response = await api.get<ProductDto>(`/products/${id}`);
        return response.data;
    },

    async getMachineryById(id: number): Promise<MachineryDto> {
        const response = await api.get<MachineryDto>(`/machinery/${id}`);
        return response.data;
    },
};
