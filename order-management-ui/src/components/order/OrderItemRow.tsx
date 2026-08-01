import React from 'react';
import { Button, Badge } from 'react-bootstrap';
import { OrderItem as OrderItemType } from '../../types';
import { formatCurrency } from '../../utils/formatters';

interface OrderItemRowProps {
  item: OrderItemType;
  onRemove: (itemId: number) => void;
  loading?: boolean;
}

export const OrderItemRow: React.FC<OrderItemRowProps> = ({ 
  item, 
  onRemove, 
  loading = false 
}) => {
  return (
    <tr>
      <td>
        <div className="d-flex align-items-center">
          <span className="fw-medium">{item.productName}</span>
        </div>
      </td>
      <td className="text-center">{item.quantity}</td>
      <td className="text-end">{formatCurrency(item.unitPrice)}</td>
      <td className="text-end fw-bold">{formatCurrency(item.lineTotal)}</td>
      <td className="text-center">
        <Button
          size="sm"
          variant="outline-danger"
          onClick={() => onRemove(item.id)}
          disabled={loading}
        >
          ✕
        </Button>
      </td>
    </tr>
  );
};