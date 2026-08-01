import React, { useState } from 'react';
import { Card, Button, Badge, Form, Row, Col } from 'react-bootstrap';
import { Product } from '../../types';
import { formatCurrency } from '../../utils/formatters';

interface ProductCardProps {
  product: Product;
  onAdd: (productId: number, quantity: number) => void;
  loading?: boolean;
}

export const ProductCard: React.FC<ProductCardProps> = ({ 
  product, 
  onAdd, 
  loading = false 
}) => {
  const [quantity, setQuantity] = useState(1);

  const handleAdd = () => {
    onAdd(product.id, quantity);
  };

  return (
    <Card className="h-100 shadow-sm">
      <Card.Body>
        <div className="d-flex justify-content-between align-items-start mb-2">
          <Card.Title className="h6 mb-0">{product.name}</Card.Title>
          <Badge bg={product.isTaxable ? 'warning' : 'success'}>
            {product.isTaxable ? 'Taxable' : 'Zero-Rated'}
          </Badge>
        </div>
        <Card.Text className="text-muted small">{product.description}</Card.Text>
        <div className="d-flex justify-content-between align-items-center mb-2">
          <span className="h5 mb-0 text-primary">{formatCurrency(product.unitPrice)}</span>
          <Badge bg="secondary">{product.category}</Badge>
        </div>
        <Row className="g-2 mt-2">
          <Col xs={5}>
            <Form.Control
              type="number"
              size="sm"
              min="1"
              max="99"
              value={quantity}
              onChange={(e) => setQuantity(Math.max(1, parseInt(e.target.value) || 1))}
              disabled={loading}
            />
          </Col>
          <Col xs={7}>
            <Button
              size="sm"
              variant="primary"
              className="w-100"
              onClick={handleAdd}
              disabled={loading}
            >
              Add to Order
            </Button>
          </Col>
        </Row>
      </Card.Body>
    </Card>
  );
};