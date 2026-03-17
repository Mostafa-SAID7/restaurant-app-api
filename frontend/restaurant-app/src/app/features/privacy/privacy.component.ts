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
      <div class="container" style="position:relative;z-index:1;padding-top:calc(var(--header-h) + var(--space-12));padding-bottom:var(--space-12);">
        <h1 class="section-title">Privacy Policy</h1>
        <div class="divider" style="margin-left:0;"></div>
        <p class="text-muted" style="margin-top:var(--space-4);">Last Updated: March 2026</p>
      </div>
    </div>

    <!-- Content -->
    <section class="section">
      <div class="container text-content">
        <h2>1. Information We Collect</h2>
        <p>When you visit Noir & Orange, make a reservation, or order online, we collect information that you voluntarily provide to us. This may include your name, email address, phone number, and payment information (handled securely by our payment processors).</p>

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
          Noir & Orange<br>
          18 West 29th Street<br>
          New York, NY 10001<br>
          <a href="mailto:privacy@noirorange.com" style="color:var(--color-accent);text-decoration:none;">privacy&#64;noirorange.com</a>
        </p>
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

      ul {
        color: var(--color-text-muted);
        line-height: 1.8;
        margin-bottom: var(--space-4);
        padding-left: var(--space-6);

        li {
          margin-bottom: var(--space-2);
        }
      }
    }
  `]
})
export class PrivacyComponent {}
