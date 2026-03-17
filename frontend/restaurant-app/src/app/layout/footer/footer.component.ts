import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

import { IconComponent } from '../../shared/components/icon.component';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [RouterLink, IconComponent],
  template: `
    <footer class="footer">
      <div class="container">
        <div class="footer-grid">
          <!-- Brand -->
          <div class="footer-brand">
            <div class="logo">
              <app-icon name="diamondFill" class="logo-icon"></app-icon>
              <span>Noir <span class="text-accent">&</span> Orange</span>
            </div>
            <p class="footer-tagline">Where every meal is a masterpiece. Experience contemporary fine dining in a noir setting.</p>
            <div class="footer-socials">
              <a href="#" aria-label="Instagram" class="social-link">
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="2" y="2" width="20" height="20" rx="5" ry="5"/><circle cx="12" cy="12" r="4"/><circle cx="17.5" cy="6.5" r="1" fill="currentColor"/></svg>
              </a>
              <a href="#" aria-label="Facebook" class="social-link">
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M18 2h-3a5 5 0 0 0-5 5v3H7v4h3v8h4v-8h3l1-4h-4V7a1 1 0 0 1 1-1h3z"/></svg>
              </a>
              <a href="#" aria-label="Twitter" class="social-link">
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M22 4s-.7 2.1-2 3.4c1.6 10-9.4 17.3-18 11.6 2.2.1 4.4-.6 6-2C3 15.5.5 9.6 3 5c2.2 2.6 5.6 4.1 9 4-.9-4.2 4-6.6 7-3.8 1.1 0 3-1.2 3-1.2z"/></svg>
              </a>
            </div>
          </div>

          <!-- Quick Links -->
          <div class="footer-col">
            <h4 class="footer-heading">Navigate</h4>
            <ul class="footer-links">
              <li><a routerLink="/">Home</a></li>
              <li><a routerLink="/menu">Our Menu</a></li>
              <li><a routerLink="/reservations">Reservations</a></li>
              <li><a routerLink="/checkout">Order Online</a></li>
              <li><a routerLink="/about">About Us</a></li>
            </ul>
          </div>

          <!-- Hours -->
          <div class="footer-col">
            <h4 class="footer-heading">Hours</h4>
            <ul class="footer-links">
              <li class="hours-row"><span>Mon – Thu</span><span>6pm – 11pm</span></li>
              <li class="hours-row"><span>Fri – Sat</span><span>6pm – 1am</span></li>
              <li class="hours-row"><span>Sunday</span><span>6pm – 10pm</span></li>
              <li class="hours-row"><span>Lunch</span><span>Fri – Sun 12pm</span></li>
            </ul>
          </div>

          <!-- Contact -->
          <div class="footer-col">
            <h4 class="footer-heading">Contact</h4>
            <ul class="footer-links">
              <li><app-icon name="mapPin" style="margin-right:2px;"></app-icon> 18 West 29th Street,<br>New York, NY 10001</li>
              <li><app-icon name="phone" style="margin-right:2px;"></app-icon> <a href="tel:+15559647642">+1 (555) NOIR</a></li>
              <li><app-icon name="envelope" style="margin-right:2px;"></app-icon> <a href="mailto:reservations@noirorange.com">reservations&#64;noirorange.com</a></li>
            </ul>

            <a routerLink="/reservations" class="btn btn-primary btn-sm footer-cta">
              Reserve a Table
            </a>
          </div>
        </div>

        <div class="footer-bottom">
          <p>© 2026 Noir & Orange. All rights reserved.</p>
          <div class="footer-legal">
            <a routerLink="/privacy">Privacy Policy</a>
            <a routerLink="/terms">Terms of Service</a>
          </div>
        </div>
      </div>
    </footer>
  `,
  styles: [`
    .footer {
      background: var(--color-surface);
      border-top: 1px solid var(--color-border);
      padding: var(--space-16) 0 var(--space-8);
      margin-top: auto;
    }

    .footer-grid {
      display: grid;
      grid-template-columns: 2fr 1fr 1fr 1.5fr;
      gap: var(--space-10);
      padding-bottom: var(--space-10);
      border-bottom: 1px solid var(--color-border);
    }

    .logo {
      display: flex;
      align-items: center;
      gap: var(--space-2);
      font-family: var(--font-serif);
      font-size: 1.3rem;
      font-weight: 600;
      color: var(--color-text);
      margin-bottom: var(--space-4);
    }

    .logo-icon { color: var(--color-accent); }

    .footer-tagline {
      font-size: 0.9rem;
      color: var(--color-text-muted);
      line-height: 1.7;
      margin-bottom: var(--space-5);
    }

    .footer-socials {
      display: flex;
      gap: var(--space-3);
    }

    .social-link {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 36px;
      height: 36px;
      border-radius: var(--radius-md);
      background: var(--color-surface-2);
      border: 1px solid var(--color-border);
      color: var(--color-text-muted);
      text-decoration: none;
      transition: all var(--transition-fast);

      &:hover {
        color: var(--color-accent);
        border-color: var(--color-accent);
        background: var(--color-accent-muted);
      }
    }

    .footer-heading {
      font-family: var(--font-sans);
      font-size: 0.75rem;
      font-weight: 700;
      letter-spacing: 0.12em;
      text-transform: uppercase;
      color: var(--color-accent);
      margin-bottom: var(--space-4);
    }

    .footer-links {
      list-style: none;
      display: flex;
      flex-direction: column;
      gap: var(--space-3);

      li {
        font-size: 0.9rem;
        color: var(--color-text-muted);
        line-height: 1.5;

        a {
          color: inherit;
          text-decoration: none;
          transition: color var(--transition-fast);

          &:hover { color: var(--color-accent); }
        }
      }
    }

    .hours-row {
      display: flex;
      justify-content: space-between;
      gap: var(--space-4);
    }

    .footer-cta {
      margin-top: var(--space-5);
    }

    .footer-bottom {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding-top: var(--space-6);
      font-size: 0.85rem;
      color: var(--color-text-dim);
    }

    .footer-legal {
      display: flex;
      gap: var(--space-4);

      a {
        color: var(--color-text-dim);
        text-decoration: none;
        transition: color var(--transition-fast);

        &:hover { color: var(--color-accent); }
      }
    }

    @media (max-width: 1024px) {
      .footer-grid { grid-template-columns: 1fr 1fr; }
      .footer-brand { grid-column: 1 / -1; }
    }

    @media (max-width: 600px) {
      .footer-grid { grid-template-columns: 1fr; }
      .footer-bottom { flex-direction: column; gap: var(--space-3); text-align: center; }
    }
  `]
})
export class FooterComponent {}
