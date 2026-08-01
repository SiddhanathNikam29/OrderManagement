import React, { useState } from 'react';
import { Row, Col, Form, Button, Badge } from 'react-bootstrap';
import { formatCurrency } from '../../utils/formatters';

interface DiscountControlProps {
  onApplyDiscount: (type: 'Percentage' | 'Fixed', value: number) => void;
  onRemoveDiscount: () => void;
  currentDiscount?: {
    type: string | null;
    amount: number;
    value: number | null;
  };
  loading?: boolean;
}

export const DiscountControl: React.FC<DiscountControlProps> = ({
  onApplyDiscount,
  onRemoveDiscount,
  currentDiscount,
  loading = false,
}) => {
  const [discountType, setDiscountType] = useState<'Percentage' | 'Fixed'>('Percentage');
  const [discountValue, setDiscountValue] = useState<string>('');

  const handleApply = () => {
    const value = parseFloat(discountValue);
    if (isNaN(value) || value <= 0) {
      alert('Please enter a valid discount value');
      return;
    }
    onApplyDiscount(discountType, value);
    setDiscountValue('');
  };

  const hasDiscount = currentDiscount && currentDiscount.amount > 0;

  return (
    <div className="p-3 bg-light rounded">
      <Row className="g-2 align-items-end">
        <Col xs={4} lg={3}>
          <Form.Label className="small mb-0">Type</Form.Label>
          <Form.Select
            size="sm"
            value={discountType}
            onChange={(e) => setDiscountType(e.target.value as 'Percentage' | 'Fixed')}
            disabled={loading || hasDiscount}
          >
            <option value="Percentage">Percentage (%)</option>
            <option value="Fixed">Fixed Amount ($)</option>
          </Form.Select>
        </Col>
        <Col xs={4} lg={3}>
          <Form.Label className="small mb-0">Value</Form.Label>
          <Form.Control
            type="number"
            size="sm"
            placeholder={discountType === 'Percentage' ? '10' : '10.00'}
            value={discountValue}
            onChange={(e) => setDiscountValue(e.target.value)}
            min="0"
            step={discountType === 'Percentage' ? '1' : '0.01'}
            disabled={loading || hasDiscount}
          />
        </Col>
        <Col xs={4} lg={3}>
          <Button
            size="sm"
            variant="primary"
            className="w-100"
            onClick={handleApply}
            disabled={loading || hasDiscount || !discountValue}
          >
            Apply
          </Button>
        </Col>
        {hasDiscount && (
          <Col xs={12} lg={3}>
            <Button
              size="sm"
              variant="outline-danger"
              className="w-100"
              onClick={onRemoveDiscount}
              disabled={loading}
            >
              Remove Discount
            </Button>
          </Col>
        )}
      </Row>

      {hasDiscount && (
        <div className="mt-2">
          <Badge bg="success" className="me-2">
            {currentDiscount.type} Discount
          </Badge>
          <Badge bg="info">
            Amount: {formatCurrency(currentDiscount.amount)}
          </Badge>
          {currentDiscount.value && (
            <Badge bg="secondary" className="ms-2">
              {currentDiscount.type === 'Percentage' 
                ? `${currentDiscount.value}%` 
                : formatCurrency(currentDiscount.value)}
            </Badge>
          )}
        </div>
      )}
    </div>
  );
};