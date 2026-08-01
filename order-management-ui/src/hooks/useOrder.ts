import { useState, useCallback, useEffect } from 'react';
import { Order, OrderSummary, OrderSplitResult } from '../types';
import { orderApi } from '../api/orderApi';

export const useOrder = (orderId?: number) => {
  const [order, setOrder] = useState<Order | null>(null);
  const [summary, setSummary] = useState<OrderSummary | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadOrder = useCallback(async (id: number) => {
    try {
      setLoading(true);
      setError(null);
      const [orderData, summaryData] = await Promise.all([
        orderApi.getOrder(id),
        orderApi.getOrderSummary(id),
      ]);
      setOrder(orderData);
      setSummary(summaryData);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load order');
    } finally {
      setLoading(false);
    }
  }, []);

  const createOrder = useCallback(async (customerName: string, customerEmail?: string) => {
    try {
      setLoading(true);
      setError(null);
      const newOrder = await orderApi.createOrder({ customerName, customerEmail });
      setOrder(newOrder);
      return newOrder;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create order');
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  const addItem = useCallback(async (productId: number, quantity: number) => {
    if (!order) throw new Error('No order selected');
    try {
      setLoading(true);
      setError(null);
      const updatedOrder = await orderApi.addItem(order.id, productId, quantity);
      setOrder(updatedOrder);
      const summaryData = await orderApi.getOrderSummary(order.id);
      setSummary(summaryData);
      return updatedOrder;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to add item');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [order]);

  const removeItem = useCallback(async (itemId: number) => {
    if (!order) throw new Error('No order selected');
    try {
      setLoading(true);
      setError(null);
      const updatedOrder = await orderApi.removeItem(order.id, itemId);
      setOrder(updatedOrder);
      const summaryData = await orderApi.getOrderSummary(order.id);
      setSummary(summaryData);
      return updatedOrder;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to remove item');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [order]);

  const applyDiscount = useCallback(async (discountType: 'Percentage' | 'Fixed', discountValue: number) => {
    if (!order) throw new Error('No order selected');
    try {
      setLoading(true);
      setError(null);
      const updatedOrder = await orderApi.applyDiscount({
        orderId: order.id,
        discountType,
        discountValue,
      });
      setOrder(updatedOrder);
      const summaryData = await orderApi.getOrderSummary(order.id);
      setSummary(summaryData);
      return updatedOrder;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to apply discount');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [order]);

  const splitOrder = useCallback(async (numberOfShares: number) => {
    if (!order) throw new Error('No order selected');
    try {
      setLoading(true);
      setError(null);
      const result = await orderApi.splitOrder({
        orderId: order.id,
        numberOfShares,
      });
      return result;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to split order');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [order]);

  const refresh = useCallback(async () => {
    if (order) {
      await loadOrder(order.id);
    }
  }, [order, loadOrder]);

  useEffect(() => {
    if (orderId) {
      loadOrder(orderId);
    }
  }, [orderId, loadOrder]);

  return {
    order,
    summary,
    loading,
    error,
    createOrder,
    addItem,
    removeItem,
    applyDiscount,
    splitOrder,
    refresh,
    loadOrder,
  };
};