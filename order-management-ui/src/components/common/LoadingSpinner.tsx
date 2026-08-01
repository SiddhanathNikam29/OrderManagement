import React from 'react';
import { Spinner } from 'react-bootstrap';

interface LoadingSpinnerProps {
  size?: 'sm';  // ✅ Only 'sm' is valid for Spinner
  message?: string;
}

export const LoadingSpinner: React.FC<LoadingSpinnerProps> = ({ 
  size = 'sm', 
  message = 'Loading...' 
}) => {
  return (
    <div className="text-center py-4">
      <Spinner animation="border" variant="primary" size={size}>
        <span className="visually-hidden">Loading...</span>
      </Spinner>
      {message && <p className="mt-2 text-muted">{message}</p>}
    </div>
  );
};