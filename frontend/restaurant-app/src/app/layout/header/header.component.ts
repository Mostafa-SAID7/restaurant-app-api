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
        <a routerLink="/" class="header-logo">
          <app-icon name="noorLogo" class="header-logo-icon"></app-icon>
          <span>NooR</span>
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
            <app-icon name="cart" strokeWidth="2"></app-icon>
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
  `
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
