import { Component, inject, signal } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CartService } from '../../core/services/cart.service';
import { OrderCheckout } from '../../core/models/order.model';
import { IconComponent } from '../../shared/components/icon.component';

type OrderStep = 'cart' | 'details' | 'payment' | 'confirmed';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterLink, DecimalPipe, IconComponent],
  template: `
    <!-- Page Header -->
    <div class="page-header">
      <div class="page-header-bg"></div>
      <div class="container" style="position:relative;z-index:1;padding-top:calc(var(--header-h) + var(--space-10));padding-bottom:var(--space-10);text-align:center;">
        <span class="section-label">Order Online</span>
        <h1 class="section-title" style="font-size:clamp(2rem,5vw,3rem);">Your Order</h1>
        <div class="divider"></div>

        <!-- Steps indicator -->
        <div class="steps-bar">
          @for (s of steps; track s.key; let i = $index) {
            <div class="step-item" [class.active]="currentStep() === s.key" [class.done]="isStepDone(s.key)">
              <div class="step-circle">
                @if (isStepDone(s.key)) { <app-icon name="check" strokeWidth="2"></app-icon> }
                @else { {{ i + 1 }} }
              </div>
              <span class="step-label">{{ s.label }}</span>
            </div>
            @if (i < steps.length - 1) {
              <div class="step-connector" [class.done]="isStepDone(s.key)"></div>
            }
          }
        </div>
      </div>
    </div>

    <div class="container section-sm">

      <!-- STEP 1: Cart -->
      @if (currentStep() === 'cart') {
        <div class="checkout-layout">
          <div class="checkout-main">
            <h2 class="step-title">Your Cart</h2>

            @if (cartService.isEmpty()) {
              <div class="empty-cart">
                <app-icon name="cart" class="empty-icon"></app-icon>
                <h3>Your cart is empty</h3>
                <p class="text-muted">Add some dishes from the menu to get started.</p>
                <a routerLink="/menu" class="btn btn-primary">Browse Menu</a>
              </div>
            } @else {
              <div class="cart-items">
                @for (item of cartService.items(); track item.menuItem.id) {
                  <div class="cart-item card">
                    <app-icon [name]="getCategoryIcon(item.menuItem.category)" class="cart-item-emoji"></app-icon>
                    <div class="cart-item-info">
                      <h4>{{ item.menuItem.name }}</h4>
                      <span class="text-muted" style="font-size:0.85rem;">{{ item.menuItem.category }}</span>
                    </div>
                    <div class="cart-item-qty">
                      <button class="qty-btn" (click)="cartService.updateQuantity(item.menuItem.id, item.quantity - 1)"><app-icon name="minus" strokeWidth="2"></app-icon></button>
                      <span class="qty-num">{{ item.quantity }}</span>
                      <button class="qty-btn" (click)="cartService.updateQuantity(item.menuItem.id, item.quantity + 1)"><app-icon name="plus" strokeWidth="2"></app-icon></button>
                    </div>
                    <div class="cart-item-price">
                      <span class="price">\${{ (item.menuItem.price * item.quantity) | number:'1.2-2' }}</span>
                      <button class="remove-btn" (click)="cartService.removeItem(item.menuItem.id)"><app-icon name="close" strokeWidth="2.5"></app-icon></button>
                    </div>
                  </div>
                }
              </div>

              <div class="cart-actions">
                <button class="btn btn-ghost btn-sm" (click)="cartService.clearCart()">Clear Cart</button>
                <a routerLink="/menu" class="btn btn-outline btn-sm">Add More Items</a>
              </div>
            }
          </div>

          @if (!cartService.isEmpty()) {
            <div class="checkout-sidebar">
              <div class="order-summary card">
                <h3>Order Summary</h3>
                <div class="summary-lines">
                  <div class="summary-line">
                    <span class="text-muted">Subtotal</span>
                    <span>\${{ cartService.totalAmount() | number:'1.2-2' }}</span>
                  </div>
                  <div class="summary-line">
                    <span class="text-muted">Service (10%)</span>
                    <span>\${{ (cartService.totalAmount() * 0.1) | number:'1.2-2' }}</span>
                  </div>
                  <div class="summary-line summary-total">
                    <span>Total</span>
                    <span class="price">\${{ (cartService.totalAmount() * 1.1) | number:'1.2-2' }}</span>
                  </div>
                </div>
                <button class="btn btn-primary btn-lg" style="width:100%;justify-content:center;" (click)="goTo('details')">
                  Proceed to Details →
                </button>
              </div>
            </div>
          }
        </div>
      }

      <!-- STEP 2: Customer Details -->
      @if (currentStep() === 'details') {
        <div class="checkout-layout">
          <div class="checkout-main">
            <h2 class="step-title">Your Details</h2>
            <form [formGroup]="detailsForm" (ngSubmit)="goTo('payment')" class="details-form">
              <div class="form-group">
                <label class="form-label">Order Type *</label>
                <div class="order-type-grid">
                  @for (type of orderTypes; track type.value) {
                    <label class="order-type-option" [class.selected]="detailsForm.value.orderType === type.value">
                      <input type="radio" formControlName="orderType" [value]="type.value" hidden>
                      <app-icon [name]="type.icon" class="type-icon"></app-icon>
                      <strong>{{ type.label }}</strong>
                      <small class="text-muted">{{ type.desc }}</small>
                    </label>
                  }
                </div>
              </div>

              <div class="form-row-2">
                <div class="form-group">
                  <label class="form-label" for="cname">Full Name *</label>
                  <input id="cname" type="text" class="form-input" formControlName="name" placeholder="John Doe">
                </div>
                <div class="form-group">
                  <label class="form-label" for="cemail">Email *</label>
                  <input id="cemail" type="email" class="form-input" formControlName="email" placeholder="john@example.com">
                </div>
              </div>

              <div class="form-group">
                <label class="form-label" for="cphone">Phone *</label>
                <input id="cphone" type="tel" class="form-input" formControlName="phone" placeholder="+1 (555) 000-0000">
              </div>

              @if (detailsForm.value.orderType === 'delivery') {
                <div class="form-group">
                  <label class="form-label" for="caddress">Delivery Address *</label>
                  <input id="caddress" type="text" class="form-input" formControlName="address" placeholder="123 Main St, New York, NY">
                </div>
              }

              <div class="form-group">
                <label class="form-label" for="cinstructions">Special Instructions</label>
                <textarea id="cinstructions" class="form-input" formControlName="specialInstructions" placeholder="Any dietary requirements or special requests..."></textarea>
              </div>

              <div class="step-actions">
                <button type="button" class="btn btn-ghost" (click)="goTo('cart')">← Back to Cart</button>
                <button type="submit" class="btn btn-primary btn-lg" [disabled]="detailsForm.invalid">Continue to Payment →</button>
              </div>
            </form>
          </div>

          <ng-container *ngTemplateOutlet="orderSummaryPanel"></ng-container>
        </div>
      }

      <!-- STEP 3: Payment -->
      @if (currentStep() === 'payment') {
        <div class="checkout-layout">
          <div class="checkout-main">
            <h2 class="step-title">Payment Details</h2>
            <form [formGroup]="paymentForm" (ngSubmit)="placeOrder()" class="details-form">
              <div class="card-visual">
                <div class="card-chip">▬</div>
                <div class="card-number-preview">**** **** **** {{ paymentForm.value.cardNumber?.slice(-4) || '****' }}</div>
                <div class="card-meta">
                  <span>{{ paymentForm.value.cardHolder || 'Card Holder' }}</span>
                  <span>{{ paymentForm.value.cardExpiry || 'MM/YY' }}</span>
                </div>
              </div>

              <div class="form-group">
                <label class="form-label" for="cardnum">Card Number *</label>
                <input id="cardnum" type="text" class="form-input" formControlName="cardNumber"
                  placeholder="1234 5678 9012 3456" maxlength="19">
              </div>

              <div class="form-row-2">
                <div class="form-group">
                  <label class="form-label" for="cardholder">Card Holder *</label>
                  <input id="cardholder" type="text" class="form-input" formControlName="cardHolder" placeholder="John Doe">
                </div>
                <div class="form-row-2" style="gap:var(--space-3);">
                  <div class="form-group">
                    <label class="form-label" for="expiry">Expiry *</label>
                    <input id="expiry" type="text" class="form-input" formControlName="cardExpiry" placeholder="MM/YY" maxlength="5">
                  </div>
                  <div class="form-group">
                    <label class="form-label" for="cvc">CVC *</label>
                    <input id="cvc" type="text" class="form-input" formControlName="cardCvc" placeholder="123" maxlength="3">
                  </div>
                </div>
              </div>

              <div class="secure-note">
                <app-icon name="lock" style="vertical-align:-0.15em; margin-right:4px;"></app-icon> Your payment info is encrypted and secure.
              </div>

              <div class="step-actions">
                <button type="button" class="btn btn-ghost" (click)="goTo('details')">← Back</button>
                <button type="submit" class="btn btn-primary btn-lg" [disabled]="paymentForm.invalid || submitting()">
                  @if (submitting()) {
                    <span class="spinner" style="width:18px;height:18px;border-width:2px;"></span> Processing...
                  } @else {
                    Place Order · \${{ (cartService.totalAmount() * 1.1) | number:'1.2-2' }}
                  }
                </button>
              </div>
            </form>
          </div>

          <ng-container *ngTemplateOutlet="orderSummaryPanel"></ng-container>
        </div>
      }

      <!-- STEP 4: Confirmed -->
      @if (currentStep() === 'confirmed') {
        <div class="confirmed-screen animate-fade-up">
          <div class="confirm-tick"><app-icon name="check" strokeWidth="2"></app-icon></div>
          <h2>Order Placed!</h2>
          <p class="text-muted">{{ orderMessage() }}</p>
          <div class="confirm-id-box">
            <span class="text-muted">Order #</span>
            <strong class="text-accent" style="font-size:1.3rem;">{{ orderId() }}</strong>
          </div>
          <div style="display:flex;gap:var(--space-4);justify-content:center;flex-wrap:wrap;margin-top:var(--space-8);">
            <a routerLink="/" class="btn btn-ghost">Back to Home</a>
            <a routerLink="/menu" class="btn btn-primary">Order Again</a>
          </div>
        </div>
      }
    </div>

    <!-- Order Summary Panel (reusable template) -->
    <ng-template #orderSummaryPanel>
      <div class="checkout-sidebar">
        <div class="order-summary card">
          <h3>Order Summary</h3>
          <div class="mini-items">
            @for (item of cartService.items(); track item.menuItem.id) {
              <div class="mini-item">
                <span>{{ item.quantity }}× {{ item.menuItem.name }}</span>
                <span class="text-muted">\${{ (item.menuItem.price * item.quantity) | number:'1.2-2' }}</span>
              </div>
            }
          </div>
          <div class="summary-lines">
            <div class="summary-line">
              <span class="text-muted">Subtotal</span>
              <span>\${{ cartService.totalAmount() | number:'1.2-2' }}</span>
            </div>
            <div class="summary-line">
              <span class="text-muted">Service (10%)</span>
              <span>\${{ (cartService.totalAmount() * 0.1) | number:'1.2-2' }}</span>
            </div>
            <div class="summary-line summary-total">
              <strong>Total</strong>
              <span class="price">\${{ (cartService.totalAmount() * 1.1) | number:'1.2-2' }}</span>
            </div>
          </div>
        </div>
      </div>
    </ng-template>
  `,
  styles: [`
    .page-header {
      background: var(--color-surface);
      border-bottom: 1px solid var(--color-border);
      overflow: hidden;
      position: relative;
    }

    .page-header-bg {
      position: absolute;
      inset: 0;
      background: radial-gradient(ellipse 60% 80% at 50% 100%, rgba(230,126,34,0.06) 0%, transparent 70%);
    }

    /* Steps bar */
    .steps-bar {
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 0;
      margin-top: var(--space-6);
    }

    .step-item {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: var(--space-2);
    }

    .step-circle {
      width: 36px;
      height: 36px;
      border-radius: 50%;
      border: 2px solid var(--color-border);
      background: var(--color-surface-2);
      color: var(--color-text-muted);
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 0.85rem;
      font-weight: 700;
      transition: all var(--transition-base);
    }

    .step-item.active .step-circle {
      border-color: var(--color-accent);
      background: var(--color-accent-muted);
      color: var(--color-accent);
    }

    .step-item.done .step-circle {
      border-color: var(--color-success);
      background: rgba(39,174,96,0.12);
      color: var(--color-success);
    }

    .step-label {
      font-size: 0.75rem;
      color: var(--color-text-muted);
      white-space: nowrap;
    }

    .step-item.active .step-label { color: var(--color-accent); font-weight: 600; }

    .step-connector {
      width: 80px;
      height: 2px;
      background: var(--color-border);
      margin: 0 var(--space-2);
      margin-bottom: var(--space-6);
      transition: background var(--transition-base);

      &.done { background: var(--color-success); }
    }

    /* Layout */
    .checkout-layout {
      display: grid;
      grid-template-columns: 1fr 340px;
      gap: var(--space-8);
      align-items: start;
    }

    .step-title {
      font-size: 1.4rem;
      margin-bottom: var(--space-6);
      padding-bottom: var(--space-4);
      border-bottom: 1px solid var(--color-border);
    }

    /* Cart Items */
    .cart-items {
      display: flex;
      flex-direction: column;
      gap: var(--space-3);
      margin-bottom: var(--space-5);
    }

    .cart-item {
      display: flex;
      align-items: center;
      gap: var(--space-4);
      padding: var(--space-4);
      border-radius: var(--radius-lg);

      &:hover { transform: none; }
    }

    .cart-item-emoji { font-size: 1.5rem; }

    .cart-item-info { flex: 1; h4 { font-size:0.95rem; } }

    .cart-item-qty {
      display: flex;
      align-items: center;
      gap: var(--space-2);
    }

    .qty-btn {
      width: 28px;
      height: 28px;
      border-radius: 50%;
      border: 1px solid var(--color-border);
      background: var(--color-surface-2);
      color: var(--color-text);
      font-size: 1rem;
      display: flex;
      align-items: center;
      justify-content: center;
      cursor: pointer;
      transition: all var(--transition-fast);

      &:hover { border-color: var(--color-accent); color: var(--color-accent); }
    }

    .qty-num { min-width: 24px; text-align: center; font-weight: 600; }

    .cart-item-price {
      display: flex;
      align-items: center;
      gap: var(--space-3);
    }

    .remove-btn {
      color: var(--color-text-dim);
      font-size: 0.75rem;
      cursor: pointer;
      background: none;
      border: none;
      padding: 4px;
      border-radius: var(--radius-sm);
      transition: color var(--transition-fast);

      &:hover { color: var(--color-error); }
    }

    .cart-actions {
      display: flex;
      gap: var(--space-3);
    }

    /* Empty Cart */
    .empty-cart {
      text-align: center;
      padding: var(--space-16) 0;
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: var(--space-4);
    }

    .empty-icon { font-size: 3rem; }

    /* Summary card */
    .order-summary {
      padding: var(--space-6);
      position: sticky;
      top: calc(var(--header-h) + var(--space-6));

      h3 { font-size: 1rem; margin-bottom: var(--space-4); }
    }

    .mini-items {
      display: flex;
      flex-direction: column;
      gap: var(--space-2);
      padding-bottom: var(--space-4);
      margin-bottom: var(--space-4);
      border-bottom: 1px solid var(--color-border);
    }

    .mini-item {
      display: flex;
      justify-content: space-between;
      font-size: 0.85rem;
      gap: var(--space-4);

      span:first-child { flex:1; color: var(--color-text-muted); }
    }

    .summary-lines {
      display: flex;
      flex-direction: column;
      gap: var(--space-3);
      margin-bottom: var(--space-5);
    }

    .summary-line {
      display: flex;
      justify-content: space-between;
      font-size: 0.9rem;
    }

    .summary-total {
      padding-top: var(--space-3);
      border-top: 1px solid var(--color-border);
      font-weight: 600;
      font-size: 1rem;
    }

    /* Details form */
    .details-form {
      display: flex;
      flex-direction: column;
      gap: var(--space-5);
    }

    .form-row-2 {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: var(--space-4);
    }

    /* Order type */
    .order-type-grid {
      display: grid;
      grid-template-columns: repeat(3, 1fr);
      gap: var(--space-3);
    }

    .order-type-option {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: var(--space-2);
      padding: var(--space-4);
      border: 2px solid var(--color-border);
      border-radius: var(--radius-lg);
      cursor: pointer;
      text-align: center;
      transition: all var(--transition-fast);

      &:hover { border-color: rgba(230,126,34,0.4); }
      &.selected { border-color: var(--color-accent); background: var(--color-accent-muted); }
    }

    .type-icon { font-size: 1.5rem; }
    .order-type-option small { font-size: 0.75rem; line-height: 1.4; }

    /* Card visual */
    .card-visual {
      background: linear-gradient(135deg, var(--color-surface-3) 0%, #2D2D2D 100%);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-xl);
      padding: var(--space-6);
      margin-bottom: var(--space-6);
      position: relative;
      overflow: hidden;

      &::before {
        content: '';
        position: absolute;
        top: 0; left: 0; right: 0; height: 3px;
        background: linear-gradient(90deg, var(--color-accent), #F39C12);
      }
    }

    .card-chip { font-size: 1.4rem; color: var(--color-accent); margin-bottom: var(--space-4); }

    .card-number-preview {
      font-size: 1.1rem;
      letter-spacing: 0.2em;
      font-family: monospace;
      color: var(--color-text-muted);
      margin-bottom: var(--space-4);
    }

    .card-meta {
      display: flex;
      justify-content: space-between;
      font-size: 0.85rem;
      color: var(--color-text-muted);
    }

    .secure-note {
      font-size: 0.83rem;
      color: var(--color-text-muted);
      padding: var(--space-3) var(--space-4);
      background: rgba(39,174,96,0.07);
      border-radius: var(--radius-md);
      border: 1px solid rgba(39,174,96,0.2);
    }

    .step-actions {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding-top: var(--space-4);
    }

    /* Confirmed */
    .confirmed-screen {
      text-align: center;
      padding: var(--space-16) 0;
      max-width: 480px;
      margin: 0 auto;
    }

    .confirm-tick {
      width: 80px;
      height: 80px;
      border-radius: 50%;
      background: rgba(39,174,96,0.1);
      border: 2px solid var(--color-success);
      color: var(--color-success);
      font-size: 2rem;
      display: flex;
      align-items: center;
      justify-content: center;
      margin: 0 auto var(--space-6);
    }

    .confirm-id-box {
      display: inline-flex;
      align-items: center;
      gap: var(--space-3);
      background: var(--color-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-lg);
      padding: var(--space-4) var(--space-6);
      margin-top: var(--space-5);
    }

    @media (max-width: 900px) {
      .checkout-layout { grid-template-columns: 1fr; }
      .order-summary { position: static; }
      .form-row-2 { grid-template-columns: 1fr; }
      .order-type-grid { grid-template-columns: 1fr 1fr; }
      .step-connector { width: 40px; }
    }
  `]
})
export class CheckoutComponent {
  cartService = inject(CartService);
  private fb  = inject(FormBuilder);

  steps = [
    { key: 'cart'      as OrderStep, label: 'Cart'     },
    { key: 'details'   as OrderStep, label: 'Details'  },
    { key: 'payment'   as OrderStep, label: 'Payment'  },
    { key: 'confirmed' as OrderStep, label: 'Confirmed'},
  ];

  stepOrder: OrderStep[] = ['cart', 'details', 'payment', 'confirmed'];
  currentStep = signal<OrderStep>('cart');
  submitting  = signal(false);
  orderId     = signal<number | null>(null);
  orderMessage = signal('');

  orderTypes = [
    { value: 'dine-in',  label: 'Dine In',  icon: 'dining', desc: 'Eat at the restaurant' },
    { value: 'takeaway', label: 'Takeaway',  icon: 'food', desc: 'Pick up your order'     },
    { value: 'delivery', label: 'Delivery',  icon: 'truck', desc: 'We deliver to your door' },
  ];

  detailsForm = this.fb.group({
    name:                ['', Validators.required],
    email:               ['', [Validators.required, Validators.email]],
    phone:               ['', Validators.required],
    orderType:           ['dine-in', Validators.required],
    address:             [''],
    specialInstructions: [''],
  });

  paymentForm = this.fb.group({
    cardNumber: ['', Validators.required],
    cardHolder: ['', Validators.required],
    cardExpiry: ['', Validators.required],
    cardCvc:    ['', Validators.required],
  });

  isStepDone(key: OrderStep): boolean {
    const cur = this.stepOrder.indexOf(this.currentStep());
    const idx = this.stepOrder.indexOf(key);
    return idx < cur;
  }

  goTo(step: OrderStep): void {
    if (step === 'payment' && this.detailsForm.invalid) {
      this.detailsForm.markAllAsTouched();
      return;
    }
    this.currentStep.set(step);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  placeOrder(): void {
    if (this.paymentForm.invalid) {
      this.paymentForm.markAllAsTouched();
      return;
    }
    this.submitting.set(true);
    const checkout: OrderCheckout = {
      ...this.detailsForm.value,
      cardNumber: this.paymentForm.value.cardNumber ?? undefined,
      cardExpiry: this.paymentForm.value.cardExpiry ?? undefined,
      cardCvc:    this.paymentForm.value.cardCvc ?? undefined,
    } as OrderCheckout;

    this.cartService.placeOrder(checkout).subscribe(res => {
      this.orderId.set(res.id);
      this.orderMessage.set(res.message);
      this.cartService.clearCart();
      this.submitting.set(false);
      this.currentStep.set('confirmed');
      window.scrollTo({ top: 0, behavior: 'smooth' });
    });
  }

  getCategoryIcon(category: string): string {
    const map: Record<string, string> = {
      Appetizers: 'category_appetizers', Mains: 'category_mains', Desserts: 'category_desserts',
      Drinks: 'category_drinks', Specials: 'category_specials'
    };
    return map[category] ?? 'food';
  }
}
