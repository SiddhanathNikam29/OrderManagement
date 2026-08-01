import React, { useState } from 'react';
import { Card, Form, Button } from 'react-bootstrap';
import { Order, OrderSplitResult } from '../../types';
import { SplitResult } from './SplitResult';
import { formatCurrency } from '../../utils/formatters';

interface SplitOrderProps {
  order: Order;
  onSplit: (shares: number) => Promise<OrderSplitResult>;
  loading?: boolean;
}

export const SplitOrder: React.FC<SplitOrderProps> = ({
  order,
  onSplit,
  loading = false,
}) => {
  const [numberOfShares, setNumberOfShares] = useState(3);
  const [splitResult, setSplitResult] = useState<OrderSplitResult | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const handleSplit = async () => {
    if (numberOfShares < 2) {
      alert('Must split into at least 2 shares');
      return;
    }

    if (order.total === 0) {
      alert('Cannot split an empty order');
      return;
    }

    setIsLoading(true);
    try {
      const result = await onSplit(numberOfShares);
      setSplitResult(result);
    } catch (error) {
      alert(error instanceof Error ? error.message : 'Failed to split order');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Card className="shadow-sm mt-3">
      <Card.Header>
        <h5 className="mb-0">🔀 Split Order (Part Two)</h5>
      </Card.Header>
      <Card.Body>
        <div className="d-flex flex-wrap gap-3 align-items-end">
          <div style={{ width: '150px' }}>
            <Form.Label className="small mb-0">Number of Shares</Form.Label>
            <Form.Control
              type="number"
              value={numberOfShares}
              onChange={(e) => setNumberOfShares(Math.max(2, parseInt(e.target.value) || 2))}
              min="2"
              max="100"
              disabled={isLoading || loading}
            />
          </div>
          <div>
            <Button
              variant="primary"
              onClick={handleSplit}
              disabled={isLoading || loading || order.total === 0}
            >
              {isLoading ? 'Splitting...' : `Split into ${numberOfShares} Shares`}
            </Button>
          </div>
          {order.total > 0 && (
            <div className="text-muted small">
              Total: {formatCurrency(order.total)}
            </div>
          )}
        </div>

        {splitResult && <SplitResult result={splitResult} />}
      </Card.Body>
    </Card>
  );
};