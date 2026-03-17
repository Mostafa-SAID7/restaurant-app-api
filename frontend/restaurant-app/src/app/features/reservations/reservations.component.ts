import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReservationService } from '../../core/services/reservation.service';
import { ReservationResponse } from '../../core/models/reservation.model';
import { IconComponent } from '../../shared/components/icon.component';

@Component({
  selector: 'app-reservations',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, IconComponent],
  template: `
    <!-- Page Header -->
    <div class="page-header">
      <div class="page-header-bg"></div>
      <div class="container" style="position:relative;z-index:1;padding-top:calc(var(--header-h) + var(--space-12));padding-bottom:var(--space-12);text-align:center;">
        <span class="section-label">Book Your Evening</span>
        <h1 class="section-title" style="font-size:clamp(2rem,5vw,3.5rem);">Reserve a Table</h1>
        <div class="divider"></div>
        <p class="section-subtitle" style="margin:var(--space-4) auto 0;">A seamless reservation experience for an evening you'll never forget.</p>
      </div>
    </div>

    <div class="container section">
      <div class="reservation-layout">

        <!-- Reservation Form -->
        <div class="reservation-form-wrap">
          @if (!confirmed()) {
            <div class="form-card card">
              <h2 class="form-card-title">Your Details</h2>
              <form [formGroup]="form" (ngSubmit)="onSubmit()" class="res-form">

                <div class="form-row-2">
                  <div class="form-group">
                    <label class="form-label" for="name">Full Name *</label>
                    <input id="name" type="text" class="form-input" formControlName="name" placeholder="John Doe"
                      [class.error]="isInvalid('name')">
                    @if (isInvalid('name')) { <span class="field-error">Full name is required</span> }
                  </div>
                  <div class="form-group">
                    <label class="form-label" for="email">Email Address *</label>
                    <input id="email" type="email" class="form-input" formControlName="email" placeholder="john@example.com"
                      [class.error]="isInvalid('email')">
                    @if (isInvalid('email')) { <span class="field-error">Valid email required</span> }
                  </div>
                </div>

                <div class="form-row-2">
                  <div class="form-group">
                    <label class="form-label" for="phone">Phone Number *</label>
                    <input id="phone" type="tel" class="form-input" formControlName="phone" placeholder="+1 (555) 000-0000"
                      [class.error]="isInvalid('phone')">
                    @if (isInvalid('phone')) { <span class="field-error">Phone number required</span> }
                  </div>
                  <div class="form-group">
                    <label class="form-label" for="guests">Number of Guests *</label>
                    <select id="guests" class="form-input" formControlName="guests">
                      @for (n of guestOptions; track n) {
                        <option [value]="n">{{ n }} {{ n === 1 ? 'Guest' : 'Guests' }}</option>
                      }
                    </select>
                  </div>
                </div>

                <div class="form-row-2">
                  <div class="form-group">
                    <label class="form-label" for="date">Date *</label>
                    <input id="date" type="date" class="form-input" formControlName="date"
                      [min]="minDate" [class.error]="isInvalid('date')">
                    @if (isInvalid('date')) { <span class="field-error">Date is required</span> }
                  </div>
                  <div class="form-group">
                    <label class="form-label" for="time">Preferred Time *</label>
                    <select id="time" class="form-input" formControlName="time">
                      @for (t of timeSlots; track t) {
                        <option [value]="t">{{ t }}</option>
                      }
                    </select>
                  </div>
                </div>

                <div class="form-group">
                  <label class="form-label" for="special">Special Requests</label>
                  <textarea id="special" class="form-input" formControlName="specialRequests"
                    placeholder="Allergies, dietary requirements, special occasion..."></textarea>
                </div>

                @if (serverError()) {
                  <p class="error-msg">{{ serverError() }}</p>
                }

                <button type="submit" class="btn btn-primary btn-lg submit-btn" [disabled]="submitting()">
                  @if (submitting()) {
                    <span class="spinner" style="width:18px;height:18px;border-width:2px;"></span>
                    Processing...
                  } @else {
                    Confirm Reservation
                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M5 12h14M12 5l7 7-7 7"/></svg>
                  }
                </button>
              </form>
            </div>
          } @else {
            <!-- Confirmation -->
            <div class="confirmation-card card animate-fade-up">
              <div class="confirm-icon"><app-icon name="check" strokeWidth="2"></app-icon></div>
              <h2>Reservation Confirmed!</h2>
              <p class="text-muted">{{ response()?.message }}</p>
              <div class="confirm-details">
                <div class="confirm-row">
                  <span class="text-muted">Confirmation #</span>
                  <strong class="text-accent">{{ response()?.id }}</strong>
                </div>
                @if (response()?.queuePosition) {
                  <div class="confirm-row">
                    <span class="text-muted">Queue Position</span>
                    <strong># {{ response()?.queuePosition }}</strong>
                  </div>
                }
                <div class="confirm-row">
                  <span class="text-muted">Date</span>
                  <strong>{{ form.value.date | date:'EEEE, MMMM d, y' }}</strong>
                </div>
                <div class="confirm-row">
                  <span class="text-muted">Time</span>
                  <strong>{{ form.value.time }}</strong>
                </div>
                <div class="confirm-row">
                  <span class="text-muted">Guests</span>
                  <strong>{{ form.value.guests }}</strong>
                </div>
              </div>
              <button class="btn btn-outline" (click)="reset()">Make Another Reservation</button>
            </div>
          }
        </div>

        <!-- Info Sidebar -->
        <div class="reservation-sidebar">
          <div class="sidebar-card card">
            <h3>Dining Information</h3>
            <ul class="info-list">
              <li>
                <app-icon name="clock" class="info-icon"></app-icon>
                <div>
                  <strong>Opening Hours</strong>
                  <p>Mon–Thu: 6pm–11pm</p>
                  <p>Fri–Sat: 6pm–1am</p>
                  <p>Sunday: 6pm–10pm</p>
                </div>
              </li>
              <li>
                <app-icon name="checkBadge" class="info-icon"></app-icon>
                <div>
                  <strong>Dress Code</strong>
                  <p>Smart casual. We ask guests to refrain from sportswear.</p>
                </div>
              </li>
              <li>
                <app-icon name="clock" class="info-icon"></app-icon>
                <div>
                  <strong>Reservation Hold</strong>
                  <p>Tables are held for 15 minutes past reservation time.</p>
                </div>
              </li>
              <li>
                <app-icon name="sparkles" class="info-icon"></app-icon>
                <div>
                  <strong>Special Occasions</strong>
                  <p>Mention your occasion in special requests and we'll make it memorable.</p>
                </div>
              </li>
            </ul>
          </div>

          <div class="sidebar-card card">
            <h3><app-icon name="mapPin" style="vertical-align:-0.1em; margin-right:4px;"></app-icon> Location</h3>
            <p class="text-muted" style="margin:var(--space-3) 0;">18 West 29th Street,<br>New York, NY 10001</p>
            <p class="text-muted">Near Penn Station. Valet parking available on weekends.</p>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .page-header {
      position: relative;
      background: var(--color-surface);
      border-bottom: 1px solid var(--color-border);
      overflow: hidden;
    }

    .page-header-bg {
      position: absolute;
      inset: 0;
      background: radial-gradient(ellipse 60% 80% at 50% 100%, rgba(230,126,34,0.06) 0%, transparent 70%);
    }

    .reservation-layout {
      display: grid;
      grid-template-columns: 1fr 360px;
      gap: var(--space-8);
      align-items: start;
    }

    .form-card {
      padding: var(--space-8);
    }

    .form-card-title {
      font-size: 1.4rem;
      margin-bottom: var(--space-6);
      padding-bottom: var(--space-4);
      border-bottom: 1px solid var(--color-border);
    }

    .res-form {
      display: flex;
      flex-direction: column;
      gap: var(--space-5);
    }

    .form-row-2 {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: var(--space-4);
    }

    .form-input.error {
      border-color: var(--color-error);
    }

    .field-error {
      font-size: 0.8rem;
      color: var(--color-error);
    }

    .error-msg {
      font-size: 0.9rem;
      color: var(--color-error);
      padding: var(--space-3) var(--space-4);
      background: rgba(231,76,60,0.08);
      border-radius: var(--radius-md);
      border: 1px solid rgba(231,76,60,0.2);
    }

    .submit-btn {
      width: 100%;
      justify-content: center;
    }

    /* Confirmation */
    .confirmation-card {
      text-align: center;
      padding: var(--space-12) var(--space-8);
    }

    .confirm-icon {
      width: 72px;
      height: 72px;
      border-radius: 50%;
      background: rgba(39,174,96,0.12);
      border: 2px solid var(--color-success);
      color: var(--color-success);
      font-size: 1.8rem;
      display: flex;
      align-items: center;
      justify-content: center;
      margin: 0 auto var(--space-5);
    }

    .confirm-details {
      background: var(--color-surface-2);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-lg);
      padding: var(--space-5);
      margin: var(--space-6) 0;
      display: flex;
      flex-direction: column;
      gap: var(--space-3);
      text-align: left;
    }

    .confirm-row {
      display: flex;
      justify-content: space-between;
      align-items: center;
      font-size: 0.9rem;
      padding-bottom: var(--space-3);
      border-bottom: 1px solid var(--color-border);

      &:last-child { border-bottom: none; padding-bottom: 0; }
    }

    /* Sidebar */
    .reservation-sidebar {
      display: flex;
      flex-direction: column;
      gap: var(--space-4);
      position: sticky;
      top: calc(var(--header-h) + var(--space-6));
    }

    .sidebar-card {
      padding: var(--space-6);

      h3 { font-size: 1rem; margin-bottom: var(--space-4); }
    }

    .info-list {
      display: flex;
      flex-direction: column;
      gap: var(--space-4);

      li {
        display: flex;
        gap: var(--space-3);
        align-items: flex-start;
      }
    }

    .info-icon { font-size: 1.3rem; flex-shrink: 0; }

    .info-list strong { display: block; font-size: 0.9rem; margin-bottom: 4px; }
    .info-list p { font-size: 0.83rem; color: var(--color-text-muted); line-height: 1.5; }

    @media (max-width: 900px) {
      .reservation-layout { grid-template-columns: 1fr; }
      .reservation-sidebar { position: static; }
      .form-row-2 { grid-template-columns: 1fr; }
    }
  `]
})
export class ReservationsComponent {
  private fb          = inject(FormBuilder);
  private resService  = inject(ReservationService);

  form       = this.fb.group({
    name:            ['', Validators.required],
    email:           ['', [Validators.required, Validators.email]],
    phone:           ['', Validators.required],
    guests:          [2, Validators.required],
    date:            ['', Validators.required],
    time:            ['19:00'],
    specialRequests: [''],
  });

  guestOptions = [1,2,3,4,5,6,7,8,9,10];
  timeSlots    = ['12:00','12:30','13:00','13:30','18:00','18:30','19:00','19:30','20:00','20:30','21:00','21:30','22:00','22:30'];
  minDate      = new Date().toISOString().split('T')[0];

  submitting  = signal(false);
  confirmed   = signal(false);
  response    = signal<ReservationResponse | null>(null);
  serverError = signal<string | null>(null);

  isInvalid(field: string): boolean {
    const ctrl = this.form.get(field);
    return !!(ctrl?.invalid && ctrl.touched);
  }

  onSubmit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.submitting.set(true);
    this.serverError.set(null);
    this.resService.makeReservation(this.form.value as any).subscribe({
      next: res => {
        this.response.set(res);
        this.confirmed.set(true);
        this.submitting.set(false);
      },
      error: () => {
        this.serverError.set('Something went wrong. Please try again.');
        this.submitting.set(false);
      }
    });
  }

  reset(): void {
    this.form.reset({ guests: 2, time: '19:00' });
    this.confirmed.set(false);
    this.response.set(null);
  }
}
