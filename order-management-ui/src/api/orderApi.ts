import { apiClient } from './client';
import { 
  Order, 
  OrderSummary, 
  OrderSplitResult, 
  CreateOrderRequest, 
  ApplyDiscountRequest,
  SplitOrderRequest 
} from '../types';

export const orderApi = {
  // Write operations (Commands)
  createOrder: (data: CreateOrderRequest) =>
    apiClient.post<Order>('/orders', data),

  addItem: (orderId: number, productId: number, quantity: number) =>
    apiClient.post<Order>(`/orders/${orderId}/items`, { orderId, productId, quantity }),

  removeItem: (orderId: number, itemId: number) =>
    apiClient.delete<Order>(`/orders/${orderId}/items/${itemId}`),

  applyDiscount: (data: ApplyDiscountRequest) =>
    apiClient.patch<Order>(`/orders/${data.orderId}/discount`, data),

  // Read operations (Queries)
  getOrder: (id: number) =>
    apiClient.get<Order>(`/orders/${id}`),

  getOrderSummary: (id: number) =>
    apiClient.get<OrderSummary>(`/orders/${id}/summary`),

  getAllOrders: (page: number = 1, pageSize: number = 10) =>
    apiClient.get<{ items: OrderSummary[]; totalCount: number; page: number; pageSize: number; totalPages: number }>(
      `/orders?page=${page}&pageSize=${pageSize}`
    ),

  splitOrder: (data: SplitOrderRequest) =>
    apiClient.post<OrderSplitResult>(`/orders/${data.orderId}/split`, data),
};