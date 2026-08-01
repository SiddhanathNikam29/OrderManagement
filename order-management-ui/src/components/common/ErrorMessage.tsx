import React from 'react';
import { Alert, Button } from 'react-bootstrap';

interface ErrorMessageProps {
  message: string;
  onRetry?: () => void;
}

export const ErrorMessage: React.FC<ErrorMessageProps> = ({ message, onRetry }) => {
  return (
    <Alert variant="danger" className="d-flex align-items-center justify-content-between">
      <span>{message}</span>
      {onRetry && (
        <Button variant="outline-danger" size="sm" onClick={onRetry}>
          Retry
        </Button>
      )}
    </Alert>
  );
};