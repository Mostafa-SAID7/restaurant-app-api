export interface Reservation {
  id?: number;
  name: string;
  email: string;
  phone: string;
  date: string;          // ISO date string
  time: string;          // e.g. "19:00"
  guests: number;
  specialRequests?: string;
  status?: 'pending' | 'confirmed' | 'cancelled';
}

export interface ReservationResponse {
  id: number;
  status: 'confirmed' | 'pending';
  queuePosition?: number;
  message: string;
}
