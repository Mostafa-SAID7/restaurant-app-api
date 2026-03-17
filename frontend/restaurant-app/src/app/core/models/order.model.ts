import { MenuItem } from './menu-item.model';

export interface CartItem {
  menuItem: MenuItem;
  quantity: number;
  notes?: string;
}

export interface Order {
  id?: number;
  items: CartItem[];
  customerName: string;
  customerEmail: string;
  customerPhone: string;
  deliveryAddress?: string;
  orderType: 'dine-in' | 'takeaway' | 'delivery';
  specialInstructions?: string;
  totalAmount: number;
  status?: 'pending' | 'confirmed' | 'preparing' | 'ready' | 'delivered';
  createdAt?: string;
}

export interface OrderCheckout {
  name: string;
  email: string;
  phone: string;
  address?: string;
  orderType: 'dine-in' | 'takeaway' | 'delivery';
  cardNumber?: string;
  cardExpiry?: string;
  cardCvc?: string;
  specialInstructions?: string;
}
