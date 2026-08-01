import React from 'react';
import { Card } from 'react-bootstrap';
import { Order } from '../../types';
import { formatCurrency } from '../../utils/formatters';

interface OrderSummaryProps {
  order: Order;
}

export const OrderSummary: React.FC<OrderSummaryProps> = ({ order }) => {
  return (
    <Card className="shadow-sm mt-3">
      <Card.Header>
        <h5 className="mb-0">💰 Order Summary</h5>
      </Card.Header>
      <Card.Body>
        <div className="summary-items">
          <div className="row py-1 border-bottom">
            <div className="col-8 text-muted">
              Subtotal ({order.items.length} items)
            </div>
            <div className="col-4 text-end">
              {formatCurrency(order.subtotal)}
            </div>
          </div>

          {order.discountAmount > 0 && (
            <div className="row py-1 border-bottom text-success">
              <div className="col-8">
                Discount
                {order.discountType && (
                  <span className="badge bg-light text-dark ms-1">
                    {order.discountType === 'Percentage' 
                      ? `${order.discountValue}%` 
                      : formatCurrency(order.discountValue || 0)}
                  </span>
                )}
              </div>
              <div className="col-4 text-end">
                -{formatCurrency(order.discountAmount)}
              </div>
            </div>
          )}

          <div className="row py-1 border-bottom text-warning">
            <div className="col-8">Tax (20%)</div>
            <div className="col-4 text-end">
              {formatCurrency(order.taxAmount)}
            </div>
          </div>

          <div className="row py-2 fs-5 fw-bold">
            <div className="col-8">Total</div>
            <div className="col-4 text-end text-primary">
              {formatCurrency(order.total)}
            </div>
          </div>
        </div>
      </Card.Body>
    </Card>
  );
};