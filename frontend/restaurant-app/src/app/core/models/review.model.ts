export interface Review {
  id: number;
  customerName: string;
  avatarUrl?: string;
  rating: number;       // 1–5
  comment: string;
  date: string;
  verified?: boolean;
}
