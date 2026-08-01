import React from 'react';
import { Card, Table, Badge, Button } from 'react-bootstrap';
import { Order } from '../../types';
import { OrderItemRow } from './OrderItemRow';
import { DiscountControl } from './DiscountControl';
import { LoadingSpinner } from '../common/LoadingSpinner';
import { formatDate } from '../../utils/formatters';

interface OrderBuilderProps {
  order: Order | null;
  loading?: boolean;
  onRemoveItem: (itemId: number) => void;
  onApplyDiscount: (type: 'Percentage' | 'Fixed', value: number) => void;
  onRemoveDiscount: () => void;
  onRefresh: () => void;
}

export const OrderBuilder: React.FC<OrderBuilderProps> = ({
  order,
  loading = false,
  onRemoveItem,
  onApplyDiscount,
  onRemoveDiscount,
  onRefresh,
}) => {
  if (loading && !order) {
    return <LoadingSpinner message="Loading order..." />;
  }

  if (!order) {
    return (
      <Card className="text-center py-5">
        <Card.Body>
          <p className="text-muted">No order selected. Create a new order to start.</p>
        </Card.Body>
      </Card>
    );
  }

  return (
    <Card className="shadow-sm">
      <Card.Header className="d-flex justify-content-between align-items-center">
        <div>
          <h5 className="mb-0">📋 Order #{order.orderNumber}</h5>
          <small className="text-muted">Created: {formatDate(order.orderDate)}</small>
        </div>
        <div className="d-flex gap-2">
          <Badge bg={order.status === 'Active' ? 'success' : 'secondary'}>
            {order.status}
          </Badge>
          <Button size="sm" variant="outline-secondary" onClick={onRefresh}>
            🔄 Refresh
          </Button>
        </div>
      </Card.Header>

      <Card.Body>
        <div className="row mb-3">
          <div className="col-md-6">
            <small className="text-muted">Customer</small>
            <p className="mb-0 fw-medium">{order.customerName || 'Guest'}</p>
            {order.customerEmail && (
              <small className="text-muted">{order.customerEmail}</small>
            )}
          </div>
          <div className="col-md-6 text-md-end">
            <small className="text-muted">Version</small>
            <p className="mb-0">v{order.version}</p>
          </div>
        </div>

        {order.items.length === 0 ? (
          <div className="text-center text-muted py-4">
            <p>🛒 Order is empty</p>
            <p className="small">Add items from the product catalogue</p>
          </div>
        ) : (
          <>
            <div className="table-responsive">
              <Table hover size="sm">
                <thead>
                  <tr>
                    <th>Product</th>
                    <th className="text-center" style={{ width: '80px' }}>Qty</th>
                    <th className="text-end" style={{ width: '100px' }}>Unit Price</th>
                    <th className="text-end" style={{ width: '120px' }}>Line Total</th>
                    <th className="text-center" style={{ width: '50px' }}></th>
                  </tr>
                </thead>
                <tbody>
                  {order.items.map(item => (
                    <OrderItemRow
                      key={item.id}
                      item={item}
                      onRemove={onRemoveItem}
                      loading={loading}
                    />
                  ))}
                </tbody>
              </Table>
            </div>

            <div className="mt-3">
              <DiscountControl
                onApplyDiscount={onApplyDiscount}
                onRemoveDiscount={onRemoveDiscount}
                currentDiscount={{
                  type: order.discountType,
                  amount: order.discountAmount,
                  value: order.discountValue,
                }}
                loading={loading}
              />
            </div>
          </>
        )}
      </Card.Body>
    </Card>
  );
};