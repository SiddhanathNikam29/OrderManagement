import React, { useState } from 'react';
import { Container, Row, Col, Button } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import '../src/App.css';

import { ProductCatalogue } from './components/catalogue/ProductCatalogue';
import { OrderBuilder } from './components/order/OrderBuilder';
import { OrderSummary } from './components/order/OrderSummary';
import { SplitOrder } from './components/split/SplitOrder';
import { CreateOrderModal } from './components/modals/CreateOrderModal';
import { LoadingSpinner } from './components/common/LoadingSpinner';
import { ErrorMessage } from './components/common/ErrorMessage';
import { useOrder } from './hooks/useOrder';
import { useProducts } from './hooks/useProducts';

function App() {
  const [showModal, setShowModal] = useState(false);

  const { 
    order, 
    loading: orderLoading, 
    error: orderError,
    createOrder,
    addItem,
    removeItem,
    applyDiscount,
    splitOrder,
    refresh,
  } = useOrder();

  const { 
    products, 
    loading: productsLoading, 
    error: productsError,
    loadProducts,
  } = useProducts();

  const handleCreateOrder = async (customerName: string, customerEmail?: string) => {
    await createOrder(customerName, customerEmail);
  };

  const handleAddItem = async (productId: number, quantity: number) => {
    await addItem(productId, quantity);
  };

  const handleRemoveItem = async (itemId: number) => {
    await removeItem(itemId);
  };

  const handleApplyDiscount = async (type: 'Percentage' | 'Fixed', value: number) => {
    await applyDiscount(type, value);
  };

  const handleRemoveDiscount = async () => {
    await applyDiscount('Percentage', 0);
  };

  const handleSplit = async (shares: number) => {
    return await splitOrder(shares);
  };

  const isLoading = orderLoading || productsLoading;

  return (
    <div className="App">
      <header className="app-header">
        <Container>
          <div className="d-flex justify-content-between align-items-center">
            <h1 className="h3 mb-0">🛒 Order Management System</h1>
            <div>
              <Button 
                size="sm" 
                variant="light"
                onClick={() => loadProducts()}
                disabled={isLoading}
              >
                🔄 Refresh Products
              </Button>
              <Button 
                size="sm" 
                variant="success" 
                className="ms-2"
                onClick={() => setShowModal(true)}
                disabled={isLoading}
              >
                + New Order
              </Button>
            </div>
          </div>
        </Container>
      </header>

      <main className="app-main">
        <Container>
          {orderError && (
            <ErrorMessage message={orderError} onRetry={refresh} />
          )}

          <Row>
            <Col lg={4} className="order-lg-2">
              {productsError && (
                <ErrorMessage 
                  message={productsError} 
                  onRetry={() => loadProducts()} 
                />
              )}
              <ProductCatalogue
                products={products}
                loading={productsLoading}
                error={productsError}
                onAddItem={handleAddItem}
                onRefresh={() => loadProducts()}
              />
            </Col>

            <Col lg={8} className="order-lg-1">
              {order ? (
                <>
                  <OrderBuilder
                    order={order}
                    loading={orderLoading}
                    onRemoveItem={handleRemoveItem}
                    onApplyDiscount={handleApplyDiscount}
                    onRemoveDiscount={handleRemoveDiscount}
                    onRefresh={refresh}
                  />
                  <OrderSummary order={order} />
                  <SplitOrder
                    order={order}
                    onSplit={handleSplit}
                    loading={orderLoading}
                  />
                </>
              ) : (
                <div className="text-center py-5 bg-white rounded shadow-sm">
                  <div className="display-1 text-muted mb-3">📋</div>
                  <h3>No Order Active</h3>
                  <p className="text-muted">
                    Click "New Order" to start building your order
                  </p>
                  <Button 
                    variant="primary" 
                    size="lg"
                    onClick={() => setShowModal(true)}
                  >
                    Create New Order
                  </Button>
                </div>
              )}
            </Col>
          </Row>
        </Container>
      </main>

      <CreateOrderModal
        show={showModal}
        onClose={() => setShowModal(false)}
        onCreate={handleCreateOrder}
        loading={isLoading}
      />
    </div>
  );
}

export default App;