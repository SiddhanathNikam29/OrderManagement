export const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(amount);
};

export const formatDate = (dateString: string): string => {
  return new Date(dateString).toLocaleString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });
};

export const getStatusBadgeClass = (status: string): string => {
  switch (status) {
    case 'Active':
      return 'bg-success';
    case 'Completed':
      return 'bg-primary';
    case 'Cancelled':
      return 'bg-danger';
    case 'Deleted':
      return 'bg-secondary';
    default:
      return 'bg-secondary';
  }
};

export const getStatusBadgeText = (status: string): string => {
  switch (status) {
    case 'Active':
      return 'Active';
    case 'Completed':
      return 'Completed';
    case 'Cancelled':
      return 'Cancelled';
    default:
      return status || 'Unknown';
  }
};