import React, { useState } from 'react';
import { Modal, Form, Button } from 'react-bootstrap';

interface CreateOrderModalProps {
  show: boolean;
  onClose: () => void;
  onCreate: (customerName: string, customerEmail?: string) => Promise<void>;
  loading?: boolean;
}

export const CreateOrderModal: React.FC<CreateOrderModalProps> = ({
  show,
  onClose,
  onCreate,
  loading = false,
}) => {
  const [customerName, setCustomerName] = useState('');
  const [customerEmail, setCustomerEmail] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!customerName.trim()) {
      alert('Customer name is required');
      return;
    }
    await onCreate(customerName.trim(), customerEmail.trim() || undefined);
    setCustomerName('');
    setCustomerEmail('');
    onClose();
  };

  return (
    <Modal show={show} onHide={onClose}>
      <Modal.Header closeButton>
        <Modal.Title>Create New Order</Modal.Title>
      </Modal.Header>
      <Form onSubmit={handleSubmit}>
        <Modal.Body>
          <Form.Group className="mb-3">
            <Form.Label>Customer Name *</Form.Label>
            <Form.Control
              type="text"
              placeholder="Enter customer name"
              value={customerName}
              onChange={(e) => setCustomerName(e.target.value)}
              required
              disabled={loading}
            />
          </Form.Group>
          <Form.Group className="mb-3">
            <Form.Label>Customer Email</Form.Label>
            <Form.Control
              type="email"
              placeholder="Enter customer email (optional)"
              value={customerEmail}
              onChange={(e) => setCustomerEmail(e.target.value)}
              disabled={loading}
            />
          </Form.Group>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={onClose} disabled={loading}>
            Cancel
          </Button>
          <Button variant="primary" type="submit" disabled={loading}>
            {loading ? 'Creating...' : 'Create Order'}
          </Button>
        </Modal.Footer>
      </Form>
    </Modal>
  );
};