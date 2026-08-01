import { apiClient } from './client';
import { Product } from '../types';

export const productApi = {
  getProducts: (category?: string, isTaxable?: boolean, search?: string) => {
    const params = new URLSearchParams();
    if (category) params.append('category', category);
    if (isTaxable !== undefined) params.append('isTaxable', String(isTaxable));
    if (search) params.append('search', search);
    const query = params.toString();
    return apiClient.get<Product[]>(`/products${query ? `?${query}` : ''}`);
  },
};