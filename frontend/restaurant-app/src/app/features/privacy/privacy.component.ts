import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-privacy',
  standalone: true,
  imports: [CommonModule],
  template: `
    <!-- Page Header -->
    <div class="page-header">
      <div class="page-header-bg"></div>
      <div class="container p-header-section">
        <h1 class="section-title">Privacy Policy</h1>
        <div class="divider ml-0"></div>
        <p class="text-muted mt-4">Last Updated: March 2026</p>
      </div>
    </div>

    <!-- Content -->
    <section class="section">
      <div class="container text-content">
        <h2>1. Information We Collect</h2>
        <p>When you visit NooR, make a reservation, or order online, we collect information that you voluntarily provide to us. This may include your name, email address, phone number, and payment information (handled securely by our payment processors).</p>

        <h2>2. How We Use Your Information</h2>
        <p>We use the information we collect to:</p>
        <ul>
          <li>Process and manage your reservations and online orders.</li>
          <li>Communicate with you regarding your dining experience.</li>
          <li>Send promotional offers (only if you have opted in).</li>
          <li>Improve our website and customer service.</li>
        </ul>

        <h2>3. Information Sharing</h2>
        <p>We do not sell, trade, or otherwise transfer your personally identifiable information to outside parties except trusted third parties who assist us in operating our website, conducting our business, or servicing you, so long as those parties agree to keep this information confidential.</p>

        <h2>4. Data Security</h2>
        <p>We implement a variety of security measures to maintain the safety of your personal information when you place an order or enter, submit, or access your personal information.</p>

        <h2>5. Contact Us</h2>
        <p>If you have any questions regarding this privacy policy, you may contact us using the information below:</p>
        <p>
          NooR<br>
          18 West 29th Street<br>
          New York, NY 10001<br>
          <a href="mailto:privacy@noor.com" class="text-accent text-decoration-none">privacy&#64;noor.com</a>
        </p>
      </div>
    </section>
  `
})
export class PrivacyComponent {}
