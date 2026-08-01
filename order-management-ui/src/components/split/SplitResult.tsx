import React from 'react';
import { Alert, Badge } from 'react-bootstrap';
import { OrderSplitResult } from '../../types';
import { formatCurrency } from '../../utils/formatters';

interface SplitResultProps {
  result: OrderSplitResult;
}

export const SplitResult: React.FC<SplitResultProps> = ({ result }) => {
  const totalMatches = result.shares.reduce((sum, s) => sum + s.amount, 0) === result.totalAmount;

  return (
    <div className="mt-3">
      <Alert variant="info">
        <div className="row">
          <div className="col-md-6">
            <small className="text-muted">Order</small>
            <p className="mb-0 fw-medium">#{result.orderNumber}</p>
          </div>
          <div className="col-md-6 text-md-end">
            <small className="text-muted">Total Amount</small>
            <p className="mb-0 fw-bold">{formatCurrency(result.totalAmount)}</p>
          </div>
        </div>
        <hr />
        <div className="text-center">
          <Badge bg="primary">
            Split into {result.numberOfShares} equal shares
          </Badge>
        </div>
      </Alert>

      <div className="d-flex flex-wrap gap-2">
        {result.shares.map((share) => (
          <div key={share.shareNumber} className="share-card p-3 border rounded text-center bg-light">
            <div className="text-muted small">Share #{share.shareNumber}</div>
            <div className="h5 mb-0 text-primary">
              {formatCurrency(share.amount)}
            </div>
          </div>
        ))}
      </div>

      <div className="mt-2 text-center text-muted small">
        ✨ Remainder distributed fairly (rounded to nearest cent)
        <br />
        <Badge bg={totalMatches ? 'success' : 'danger'} className="mt-1">
          {totalMatches ? '✓ Total matches' : '⚠️ Total mismatch'}
        </Badge>
      </div>
    </div>
  );
};