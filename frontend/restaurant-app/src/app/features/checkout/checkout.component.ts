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
      <div class="container p-header-section text-center">
        <span class="section-label">Order Online</span>
        <h1 class="section-title title-lg">Your Order</h1>
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
                      <span class="text-muted text-sm">{{ item.menuItem.category }}</span>
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
                <div class="order-type-grid">
                  <div class="order-type-option" [class.selected]="detailsForm.get('orderType')?.value === 'dine-in'" (click)="detailsForm.patchValue({orderType: 'dine-in'})">
                    <app-icon name="dining" class="type-icon"></app-icon>
                    <span>Dine-in</span>
                    <small>Reserve a table</small>
                  </div>
                  <div class="order-type-option" [class.selected]="detailsForm.get('orderType')?.value === 'takeaway'" (click)="detailsForm.patchValue({orderType: 'takeaway'})">
                    <app-icon name="takeout" class="type-icon"></app-icon>
                    <span>Takeout</span>
                    <small>Self-pickup</small>
                  </div>
                  <div class="order-type-option" [class.selected]="detailsForm.get('orderType')?.value === 'delivery'" (click)="detailsForm.patchValue({orderType: 'delivery'})">
                    <app-icon name="bike" class="type-icon"></app-icon>
                    <span>Delivery</span>
                    <small>To your door</small>
                  </div>
                </div>
                <button class="btn btn-primary btn-lg w-full justify-center" (click)="goTo('details')">
                  Proceed to Details <app-icon name="arrow_right" strokeWidth="2"></app-icon>
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
                <button type="button" class="btn btn-ghost" (click)="goTo('cart')"><app-icon name="arrow_left" strokeWidth="2"></app-icon> Back to Cart</button>
                <button type="submit" class="btn btn-primary btn-lg" [disabled]="detailsForm.invalid">Continue to Payment <app-icon name="arrow_right" strokeWidth="2"></app-icon></button>
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
                <div class="form-row-2 gap-3">
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
                <app-icon name="lock" class="icon-inline"></app-icon> Your payment info is encrypted and secure.
              </div>

              <div class="step-actions">
                <button type="button" class="btn btn-ghost" (click)="goTo('details')"><app-icon name="arrow_left" strokeWidth="2"></app-icon> Back</button>
                <button type="submit" class="btn btn-primary btn-lg" [disabled]="paymentForm.invalid || submitting()">
                  @if (submitting()) {
                    <span class="spinner spinner-sm"></span> Processing...
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
        <div class="confirmed-screen card animate-fade-up">
          <div class="confirm-tick">
            <app-icon name="check" strokeWidth="3"></app-icon>
          </div>
          <h2 class="title-lg">Order Confirmed</h2>
          <p class="text-xl mb-2">Thank you, {{ detailsForm.value.name }}!</p>
          <p class="text-muted leading-relaxed mb-6">{{ orderMessage() }}</p>

          <div class="confirm-id-box">
             <span class="text-xs text-muted font-bold uppercase tracking-wider">Order ID:</span>
             <span class="font-mono font-bold text-accent">#{{ orderId() }}</span>
          </div>

          <div class="mt-8 flex gap-4 justify-center">
            <a routerLink="/" class="btn btn-primary">Back to Home</a>
            <button class="btn btn-outline" (click)="currentStep.set('cart'); cartService.clearCart()">Order Something Else</button>
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
  `
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
