import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-terms',
  standalone: true,
  imports: [CommonModule],
  template: `
    <!-- Page Header -->
    <div class="page-header">
      <div class="page-header-bg"></div>
      <div class="container" style="position:relative;z-index:1;padding-top:calc(var(--header-h) + var(--space-12));padding-bottom:var(--space-12);">
        <h1 class="section-title">Terms of Service</h1>
        <div class="divider" style="margin-left:0;"></div>
        <p class="text-muted" style="margin-top:var(--space-4);">Last Updated: March 2026</p>
      </div>
    </div>

    <!-- Content -->
    <section class="section">
      <div class="container text-content">
        <h2>1. Agreement to Terms</h2>
        <p>By accessing or using the Noir & Orange website (the "Site"), you agree to be bound by these Terms of Service. If you disagree with any part of the terms, then you do not have permission to access the Service.</p>

        <h2>2. Reservations and Cancellations</h2>
        <p>Reservations are subject to availability. We kindly ask that you cancel your reservation at least 24 hours in advance. Failure to do so may result in a cancellation fee, as outlined during the booking process.</p>

        <h2>3. Online Orders</h2>
        <p>All orders placed through our online ordering system are subject to acceptance by Noir & Orange. Prices and availability are subject to change without notice. We reserve the right to refuse or cancel any order for any reason.</p>

        <h2>4. Dietary Requirements and Allergies</h2>
        <p>While we make every effort to accommodate dietary restrictions and allergies, we cannot guarantee that any of our dishes are completely free of allergens, as our kitchen handles nuts, gluten, dairy, and other common allergens.</p>

        <h2>5. Intellectual Property</h2>
        <p>The Site and its original content, features, and functionality are and will remain the exclusive property of Noir & Orange and its licensors. Our trademarks and trade dress may not be used in connection with any product or service without the prior written consent of Noir & Orange.</p>

        <h2>6. Changes to Terms</h2>
        <p>We reserve the right, at our sole discretion, to modify or replace these Terms at any time. By continuing to access or use our Service after those revisions become effective, you agree to be bound by the revised terms.</p>
      </div>
    </section>
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
      background: radial-gradient(ellipse 60% 80% at 0% 100%, rgba(230,126,34,0.06) 0%, transparent 70%);
    }

    .text-content {
      max-width: 800px;
      margin: 0 auto;

      h2 {
        font-size: 1.5rem;
        margin: var(--space-8) 0 var(--space-3);
        color: var(--color-text);
      }

      p {
        color: var(--color-text-muted);
        line-height: 1.8;
        margin-bottom: var(--space-4);
      }
    }
  `]
})
export class TermsComponent {}
