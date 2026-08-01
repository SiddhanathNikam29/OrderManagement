import React, { useState } from 'react';
import { Row, Col, Form, Button } from 'react-bootstrap';
import { Product } from '../../types';
import { ProductCard } from './ProductCard';
import { LoadingSpinner } from '../common/LoadingSpinner';
import { ErrorMessage } from '../common/ErrorMessage';

interface ProductCatalogueProps {
  products: Product[];
  loading?: boolean;
  error?: string | null;
  onAddItem: (productId: number, quantity: number) => void;
  onRefresh?: () => void;
}

export const ProductCatalogue: React.FC<ProductCatalogueProps> = ({
  products,
  loading = false,
  error = null,
  onAddItem,
  onRefresh,
}) => {
  const [category, setCategory] = useState<string>('');
  const [search, setSearch] = useState('');

  // ✅ FIX: Convert Set to array using Array.from() instead of spread operator
  const categories = Array.from(new Set(products.map(p => p.category)));

  const filteredProducts = products.filter(p => {
    const matchesCategory = !category || p.category === category;
    const matchesSearch = !search || 
      p.name.toLowerCase().includes(search.toLowerCase()) ||
      p.description.toLowerCase().includes(search.toLowerCase());
    return matchesCategory && matchesSearch;
  });

  if (loading) {
    return <LoadingSpinner message="Loading products..." />;
  }

  if (error) {
    return <ErrorMessage message={error} onRetry={onRefresh} />;
  }

  return (
    <div className="product-catalogue">
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h5 className="mb-0">📦 Product Catalogue</h5>
        <Button size="sm" variant="outline-secondary" onClick={onRefresh}>
          🔄 Refresh
        </Button>
      </div>

      <Row className="g-2 mb-3">
        <Col xs={7}>
          <Form.Control
            type="text"
            size="sm"
            placeholder="Search products..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
        </Col>
        <Col xs={5}>
          <Form.Select
            size="sm"
            value={category}
            onChange={(e) => setCategory(e.target.value)}
          >
            <option value="">All Categories</option>
            {categories.map(cat => (
              <option key={cat} value={cat}>{cat}</option>
            ))}
          </Form.Select>
        </Col>
      </Row>

      {filteredProducts.length === 0 ? (
        <div className="text-center text-muted py-4">
          <p>No products found</p>
        </div>
      ) : (
        <Row xs={1} md={2} className="g-3">
          {filteredProducts.map(product => (
            <Col key={product.id}>
              <ProductCard
                product={product}
                onAdd={onAddItem}
                loading={loading}
              />
            </Col>
          ))}
        </Row>
      )}
    </div>
  );
};