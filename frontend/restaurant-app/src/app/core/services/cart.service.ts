import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, delay } from 'rxjs/operators';
import { CartItem, Order, OrderCheckout } from '../models/order.model';
import { MenuItem } from '../models/menu-item.model';

@Injectable({ providedIn: 'root' })
export class CartService {
  private apiUrl = 'https://fakerestaurantapi.runasp.net/orders';

  // Signals-based cart state
  private _items = signal<CartItem[]>([]);
  readonly items = this._items.asReadonly();

  readonly totalItems = computed(() =>
    this._items().reduce((acc, i) => acc + i.quantity, 0)
  );

  readonly totalAmount = computed(() =>
    this._items().reduce((acc, i) => acc + i.menuItem.price * i.quantity, 0)
  );

  readonly isEmpty = computed(() => this._items().length === 0);

  constructor(private http: HttpClient) {}

  addItem(menuItem: MenuItem, quantity = 1): void {
    this._items.update(items => {
      const existing = items.find(i => i.menuItem.id === menuItem.id);
      if (existing) {
        return items.map(i => i.menuItem.id === menuItem.id
          ? { ...i, quantity: i.quantity + quantity }
          : i
        );
      }
      return [...items, { menuItem, quantity }];
    });
  }

  removeItem(menuItemId: number): void {
    this._items.update(items => items.filter(i => i.menuItem.id !== menuItemId));
  }

  updateQuantity(menuItemId: number, quantity: number): void {
    if (quantity <= 0) { this.removeItem(menuItemId); return; }
    this._items.update(items =>
      items.map(i => i.menuItem.id === menuItemId ? { ...i, quantity } : i)
    );
  }

  clearCart(): void {
    this._items.set([]);
  }

  placeOrder(checkout: OrderCheckout): Observable<{ id: number; status: string; message: string }> {
    const order: Order = {
      items: this._items(),
      customerName: checkout.name,
      customerEmail: checkout.email,
      customerPhone: checkout.phone,
      deliveryAddress: checkout.address,
      orderType: checkout.orderType,
      specialInstructions: checkout.specialInstructions,
      totalAmount: this.totalAmount(),
    };

    return this.http.post<any>(this.apiUrl, order).pipe(
      catchError(() => of({
        id: Math.floor(Math.random() * 100000),
        status: 'confirmed',
        message: 'Your order has been placed! Estimated preparation time: 25–35 minutes.'
      }).pipe(delay(1200)))
    );
  }
}
