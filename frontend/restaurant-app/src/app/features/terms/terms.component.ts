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
      <div class="container p-header-section">
        <h1 class="section-title">Terms of Service</h1>
        <div class="divider ml-0"></div>
        <p class="text-muted mt-4">Last Updated: March 2026</p>
      </div>
    </div>

    <!-- Content -->
    <section class="section">
      <div class="container text-content">
        <h2>1. Agreement to Terms</h2>
        <p>By accessing or using the NooR website (the "Site"), you agree to be bound by these Terms of Service. If you disagree with any part of the terms, then you do not have permission to access the Service.</p>

        <h2>2. Reservations and Cancellations</h2>
        <p>Reservations are subject to availability. We kindly ask that you cancel your reservation at least 24 hours in advance. Failure to do so may result in a cancellation fee, as outlined during the booking process.</p>

        <h2>3. Online Orders</h2>
        <p>All orders placed through our online ordering system are subject to acceptance by NooR. Prices and availability are subject to change without notice. We reserve the right to refuse or cancel any order for any reason.</p>

        <h2>4. Dietary Requirements and Allergies</h2>
        <p>While we make every effort to accommodate dietary restrictions and allergies, we cannot guarantee that any of our dishes are completely free of allergens, as our kitchen handles nuts, gluten, dairy, and other common allergens.</p>

        <h2>5. Intellectual Property</h2>
        <p>The Site and its original content, features, and functionality are and will remain the exclusive property of NooR and its licensors. Our trademarks and trade dress may not be used in connection with any product or service without the prior written consent of NooR.</p>

        <h2>6. Changes to Terms</h2>
        <p>We reserve the right, at our sole discretion, to modify or replace these Terms at any time. By continuing to access or use our Service after those revisions become effective, you agree to be bound by the revised terms.</p>
      </div>
    </section>
  `
})
export class TermsComponent {}
