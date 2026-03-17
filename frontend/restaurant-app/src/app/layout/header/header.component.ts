import { Component, inject, signal, HostListener } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { CartService } from '../../core/services/cart.service';
import { IconComponent } from '../../shared/components/icon.component';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, CommonModule, IconComponent],
  template: `
    <header class="header" [class.scrolled]="isScrolled()">
      <div class="container header-inner">
        <!-- Logo -->
        <a routerLink="/" class="logo">
          <app-icon name="diamondFill" class="logo-icon"></app-icon>
          <span>Noir <span class="text-accent">&</span> Orange</span>
        </a>

        <!-- Desktop Nav -->
        <nav class="nav" [class.open]="menuOpen()">
          <a routerLink="/" routerLinkActive="active" [routerLinkActiveOptions]="{exact:true}" class="nav-link" (click)="menuOpen.set(false)">Home</a>
          <a routerLink="/menu" routerLinkActive="active" class="nav-link" (click)="menuOpen.set(false)">Menu</a>
          <a routerLink="/reservations" routerLinkActive="active" class="nav-link" (click)="menuOpen.set(false)">Reserve</a>
          <a routerLink="/checkout" routerLinkActive="active" class="nav-link" (click)="menuOpen.set(false)">Order Online</a>
          <a routerLink="/about" routerLinkActive="active" class="nav-link" (click)="menuOpen.set(false)">About</a>
        </nav>

        <!-- Actions -->
        <div class="header-actions">
          <!-- Cart Button -->
          <a routerLink="/checkout" class="cart-btn" aria-label="View cart">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <circle cx="9" cy="21" r="1"/><circle cx="20" cy="21" r="1"/>
              <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"/>
            </svg>
            @if (cartCount() > 0) {
              <span class="cart-badge">{{ cartCount() }}</span>
            }
          </a>

          <!-- Reserve CTA -->
          <a routerLink="/reservations" class="btn btn-primary btn-sm header-cta" (click)="menuOpen.set(false)">
            Reserve a Table
          </a>

          <!-- Mobile hamburger -->
          <button class="hamburger" [class.is-open]="menuOpen()" (click)="menuOpen.set(!menuOpen())" aria-label="Toggle menu">
            <span></span>
            <span></span>
            <span></span>
          </button>
        </div>
      </div>
    </header>
  `,
  styles: [`
    .header {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      z-index: 1000;
      height: var(--header-h);
      background: rgba(10,10,10,0.6);
      backdrop-filter: blur(20px);
      -webkit-backdrop-filter: blur(20px);
      border-bottom: 1px solid transparent;
      transition: all var(--transition-base);
    }

    .header.scrolled {
      background: rgba(10,10,10,0.92);
      border-bottom-color: var(--color-border);
      box-shadow: 0 4px 24px rgba(0,0,0,0.4);
    }

    .header-inner {
      display: flex;
      align-items: center;
      justify-content: space-between;
      height: 100%;
      gap: var(--space-6);
    }

    .logo {
      display: flex;
      align-items: center;
      gap: var(--space-2);
      font-family: var(--font-serif);
      font-size: 1.3rem;
      font-weight: 600;
      color: var(--color-text);
      text-decoration: none;
      flex-shrink: 0;
      transition: opacity var(--transition-fast);

      &:hover { opacity: 0.85; }
    }

    .logo-icon {
      color: var(--color-accent);
      font-size: 1.1rem;
      animation: pulse-glow 3s ease-in-out infinite;
    }

    .nav {
      display: flex;
      align-items: center;
      gap: var(--space-1);
    }

    .nav-link {
      padding: var(--space-2) var(--space-3);
      border-radius: var(--radius-sm);
      font-size: 0.9rem;
      font-weight: 500;
      color: var(--color-text-muted);
      text-decoration: none;
      transition: all var(--transition-fast);
      position: relative;

      &:hover { color: var(--color-text); background: rgba(255,255,255,0.05); }
      &.active { color: var(--color-accent); }

      &.active::after {
        content: '';
        position: absolute;
        bottom: 0;
        left: 50%;
        transform: translateX(-50%);
        width: 16px;
        height: 2px;
        background: var(--color-accent);
        border-radius: var(--radius-full);
      }
    }

    .header-actions {
      display: flex;
      align-items: center;
      gap: var(--space-3);
    }

    .cart-btn {
      position: relative;
      display: flex;
      align-items: center;
      justify-content: center;
      width: 40px;
      height: 40px;
      border-radius: var(--radius-md);
      color: var(--color-text-muted);
      background: rgba(255,255,255,0.05);
      border: 1px solid var(--color-border);
      text-decoration: none;
      transition: all var(--transition-fast);

      &:hover { color: var(--color-accent); border-color: var(--color-accent); }
    }

    .cart-badge {
      position: absolute;
      top: -6px;
      right: -6px;
      background: var(--color-accent);
      color: white;
      font-size: 0.65rem;
      font-weight: 700;
      width: 18px;
      height: 18px;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .hamburger {
      display: none;
      flex-direction: column;
      gap: 5px;
      width: 28px;
      padding: 4px;
      background: none;
      border: none;
      cursor: pointer;

      span {
        display: block;
        height: 2px;
        background: var(--color-text);
        border-radius: 2px;
        transition: all var(--transition-fast);

      &.is-open span:nth-child(1) { transform: translateY(7px) rotate(45deg); }
      &.is-open span:nth-child(2) { opacity: 0; }
      &.is-open span:nth-child(3) { transform: translateY(-7px) rotate(-45deg); }
      }
    }

    @media (max-width: 900px) {
      .header-cta { display: none; }

      .nav {
        position: fixed;
        top: var(--header-h);
        left: 0;
        right: 0;
        flex-direction: column;
        align-items: stretch;
        background: rgba(10,10,10,0.97);
        backdrop-filter: blur(20px);
        padding: var(--space-4);
        gap: var(--space-2);
        border-bottom: 1px solid var(--color-border);
        transform: translateY(-100%);
        opacity: 0;
        pointer-events: none;
        transition: all var(--transition-base);

        &.open {
          transform: translateY(0);
          opacity: 1;
          pointer-events: all;
        }
      }

      .nav-link {
        padding: var(--space-3) var(--space-4);
        font-size: 1.05rem;
        border-radius: var(--radius-md);
      }

      .hamburger { display: flex; }
    }
  `]
})
export class HeaderComponent {
  private cart = inject(CartService);

  cartCount  = this.cart.totalItems;
  isScrolled = signal(false);
  menuOpen   = signal(false);

  @HostListener('window:scroll')
  onScroll(): void {
    this.isScrolled.set(window.scrollY > 20);
  }
}
