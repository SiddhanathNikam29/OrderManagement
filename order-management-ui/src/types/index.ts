export interface Product {
  id: number;
  name: string;
  description: string;
  unitPrice: number;
  isTaxable: boolean;
  category: string;
  createdAt: string;
  taxStatus: string;
}

export interface OrderItem {
  id: number;
  productId: number;
  productName: string;
  unitPrice: number;
  quantity: number;
  lineTotal: number;
}

export interface Order {
  id: number;
  orderNumber: string;
  customerName: string;
  customerEmail: string;
  orderDate: string;
  items: OrderItem[];
  subtotal: number;
  discountType: string | null;
  discountValue: number | null;
  discountAmount: number;
  taxableAmount: number;
  taxAmount: number;
  total: number;
  status: string;
  version: number;
  updatedAt: string;
}

export interface OrderSummary {
  orderId: number;
  orderNumber: string;
  customerName: string;
  customerEmail: string;
  orderDate: string;
  itemCount: number;
  subtotal: number;
  discountType: string | null;
  discountValue: number | null;
  discountAmount: number;
  taxableAmount: number;
  taxAmount: number;
  total: number;
  status: string;
  version: number;
  updatedAt: string;
  discountDisplayValue: number;
  discountSymbol: string;
}

export interface OrderShare {
  shareNumber: number;
  amount: number;
}

export interface OrderSplitResult {
  orderId: number;
  orderNumber: string;
  totalAmount: number;
  numberOfShares: number;
  shares: OrderShare[];
}

export interface CreateOrderRequest {
  customerName: string;
  customerEmail?: string;
}

export interface AddItemRequest {
  orderId: number;
  productId: number;
  quantity: number;
}

export interface ApplyDiscountRequest {
  orderId: number;
  discountType: 'Percentage' | 'Fixed';
  discountValue: number;
}

export interface SplitOrderRequest {
  orderId: number;
  numberOfShares: number;
}

export interface ApiResponse<T> {
  data: T;
  message?: string;
  error?: string;
}