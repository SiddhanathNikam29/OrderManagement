import { useState, useEffect, useCallback } from 'react';
import { Product } from '../types';
import { productApi } from '../api/productApi';

export const useProducts = () => {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadProducts = useCallback(async (category?: string, isTaxable?: boolean, search?: string) => {
    try {
      setLoading(true);
      setError(null);
      const data = await productApi.getProducts(category, isTaxable, search);
      setProducts(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load products');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadProducts();
  }, [loadProducts]);

  return {
    products,
    loading,
    error,
    loadProducts,
  };
};