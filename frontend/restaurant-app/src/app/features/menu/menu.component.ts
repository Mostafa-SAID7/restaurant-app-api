import { Component, OnInit, signal, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MenuService } from '../../core/services/menu.service';
import { CartService } from '../../core/services/cart.service';
import { MenuItem, MenuCategory } from '../../core/models/menu-item.model';
import { IconComponent } from '../../shared/components/icon.component';

type FilterCategory = 'All' | MenuCategory;

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, IconComponent],
  template: `
    <!-- Page Header -->
    <div class="page-header">
      <div class="page-header-bg"></div>
      <div class="container" style="position:relative; z-index:1; padding-top:calc(var(--header-h) + var(--space-12)); padding-bottom:var(--space-12);">
        <span class="section-label">Our Culinary Offerings</span>
        <h1 class="section-title" style="text-align:center; font-size:clamp(2rem,5vw,3.5rem);">The Full Menu</h1>
        <div class="divider"></div>
        <p class="section-subtitle" style="text-align:center; margin:var(--space-4) auto 0;">Every dish is composed with seasonal ingredients, artisan techniques, and a deep passion for flavour.</p>
      </div>
    </div>

    <div class="container" style="padding-top:var(--space-8); padding-bottom:var(--space-20);">

      <!-- Search & Filter Bar -->
      <div class="menu-controls">
        <div class="search-wrapper">
          <svg class="search-icon" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="11" cy="11" r="8"/><path d="m21 21-4.35-4.35"/></svg>
          <input
            type="text"
            class="form-input search-input"
            placeholder="Search dishes, ingredients..."
            [(ngModel)]="searchQuery"
            (ngModelChange)="onSearch($event)"
          />
          @if (searchQuery) {
            <button class="search-clear" (click)="clearSearch()"><app-icon name="close"></app-icon></button>
          }
        </div>

        <div class="category-tabs">
          @for (cat of categories; track cat) {
            <button
              class="category-tab"
              [class.active]="selectedCategory() === cat"
              (click)="setCategory(cat)"
            >
              <app-icon [name]="getCategoryIcon(cat)" style="margin-right:4px;"></app-icon> {{ cat }}
            </button>
          }
        </div>
      </div>

      <!-- Results count -->
      <div class="results-info">
        <span class="text-muted">{{ filteredItems().length }} dishes</span>
        @if (searchQuery) {
          <span class="text-muted"> · Searching for "{{ searchQuery }}"</span>
        }
      </div>

      <!-- Loading -->
      @if (loading()) {
        <div style="display:flex;justify-content:center;padding:var(--space-16) 0;">
          <div class="spinner"></div>
        </div>
      } @else {

        <!-- Menu by Category or All -->
        @if (selectedCategory() === 'All' && !searchQuery) {
          @for (cat of menuCategories; track cat) {
            @if (getItemsByCategory(cat).length > 0) {
              <section class="menu-section">
                <div class="menu-section-header">
                  <h2 class="menu-section-title">
                    <app-icon [name]="getCategoryIcon(cat)" class="menu-section-icon"></app-icon>
                    {{ cat }}
                  </h2>
                  <div class="menu-section-line"></div>
                </div>
                <div class="menu-grid">
                  @for (item of getItemsByCategory(cat); track item.id) {
                    <ng-container *ngTemplateOutlet="menuItemCard; context: { item: item }"></ng-container>
                  }
                </div>
              </section>
            }
          }
        } @else {
          <!-- Flat filtered list -->
          @if (filteredItems().length === 0) {
            <div class="empty-state">
              <p><app-icon name="search" style="font-size:1.5rem; vertical-align:-0.2em;"></app-icon> No dishes found for "{{ searchQuery }}"</p>
              <button class="btn btn-ghost" (click)="clearSearch()">Clear Search</button>
            </div>
          } @else {
            <div class="menu-grid" style="margin-top:var(--space-6);">
              @for (item of filteredItems(); track item.id) {
                <ng-container *ngTemplateOutlet="menuItemCard; context: { item: item }"></ng-container>
              }
            </div>
          }
        }
      }

      <!-- Cart Summary Sticky -->
      @if (cartCount() > 0) {
        <div class="cart-stick">
          <div class="cart-stick-inner">
            <span><app-icon name="cart"></app-icon> {{ cartCount() }} item(s) · <strong class="text-accent">\${{ cartTotal() | number:'1.2-2' }}</strong></span>
            <a routerLink="/checkout" class="btn btn-primary btn-sm">Proceed to Checkout</a>
          </div>
        </div>
      }
    </div>

    <!-- Menu Item Card Template -->
    <ng-template #menuItemCard let-item="item">
      <article class="menu-item-card card" [class.featured]="item.isFeatured">
        <div class="menu-item-image">
          <div class="image-placeholder"><app-icon [name]="getCategoryIcon(item.category)"></app-icon></div>
          @if (item.isFeatured) {
            <div class="featured-ribbon">Chef's Pick</div>
          }
          <div class="item-tags">
            @for (tag of (item.tags ?? []); track tag) {
              <span class="badge badge-accent">{{ tag }}</span>
            }
          </div>
        </div>
        <div class="menu-item-body">
          <div class="menu-item-top">
            <h3 class="menu-item-name">{{ item.name }}</h3>
            <span class="price">\${{ item.price }}</span>
          </div>
          <p class="menu-item-desc">{{ item.description }}</p>
          <div class="menu-item-footer">
            <span class="badge badge-accent" style="font-size:0.7rem;">{{ item.category }}</span>
            <button
              class="btn btn-primary btn-sm"
              [disabled]="!item.isAvailable"
              (click)="addToCart(item)"
            >
              {{ item.isAvailable ? '+ Add to Order' : 'Unavailable' }}
            </button>
          </div>
        </div>
      </article>
    </ng-template>
  `,
  styles: [`
    .page-header {
      position: relative;
      text-align: center;
      background: var(--color-surface);
      border-bottom: 1px solid var(--color-border);
      overflow: hidden;
    }

    .page-header-bg {
      position: absolute;
      inset: 0;
      background: radial-gradient(ellipse 60% 80% at 50% 100%, rgba(230,126,34,0.06) 0%, transparent 70%);
    }

    /* Controls */
    .menu-controls {
      display: flex;
      flex-direction: column;
      gap: var(--space-4);
      margin-bottom: var(--space-4);
    }

    .search-wrapper {
      position: relative;
      max-width: 480px;
    }

    .search-icon {
      position: absolute;
      left: var(--space-4);
      top: 50%;
      transform: translateY(-50%);
      color: var(--color-text-muted);
    }

    .search-input {
      padding-left: 3rem;
      padding-right: 3rem;
      border-radius: var(--radius-full);
    }

    .search-clear {
      position: absolute;
      right: var(--space-3);
      top: 50%;
      transform: translateY(-50%);
      background: none;
      border: none;
      color: var(--color-text-muted);
      cursor: pointer;
      font-size: 0.85rem;

      &:hover { color: var(--color-accent); }
    }

    .category-tabs {
      display: flex;
      gap: var(--space-2);
      flex-wrap: wrap;
    }

    .category-tab {
      padding: var(--space-2) var(--space-4);
      border-radius: var(--radius-full);
      font-size: 0.85rem;
      font-weight: 500;
      border: 1px solid var(--color-border);
      background: var(--color-surface);
      color: var(--color-text-muted);
      transition: all var(--transition-fast);
      cursor: pointer;

      &:hover { border-color: var(--color-accent); color: var(--color-text); }
      &.active { background: var(--color-accent); border-color: var(--color-accent); color: white; }
    }

    .results-info {
      font-size: 0.85rem;
      margin-bottom: var(--space-6);
    }

    /* Menu Sections */
    .menu-section { margin-bottom: var(--space-12); }

    .menu-section-header {
      display: flex;
      align-items: center;
      gap: var(--space-4);
      margin-bottom: var(--space-6);
    }

    .menu-section-title {
      display: flex;
      align-items: center;
      gap: var(--space-3);
      font-size: 1.5rem;
      white-space: nowrap;
    }

    .menu-section-icon { font-size: 1.3rem; }

    .menu-section-line {
      flex: 1;
      height: 1px;
      background: var(--color-border);
    }

    /* Menu Grid */
    .menu-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
      gap: var(--space-5);
    }

    .menu-item-card {
      display: flex;
      flex-direction: column;

      &.featured {
        border-color: rgba(230,126,34,0.3);
        box-shadow: 0 0 0 1px rgba(230,126,34,0.1);
      }
    }

    .menu-item-image {
      position: relative;
      height: 180px;
      overflow: hidden;
    }

    .image-placeholder {
      width: 100%;
      height: 100%;
      background: linear-gradient(135deg, var(--color-surface-2), var(--color-surface-3));
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 2.5rem;
      transition: transform var(--transition-slow);
    }

    .menu-item-card:hover .image-placeholder { transform: scale(1.05); }

    .featured-ribbon {
      position: absolute;
      top: var(--space-3);
      right: -1px;
      background: var(--color-accent);
      color: white;
      font-size: 0.7rem;
      font-weight: 700;
      padding: 3px 10px;
      text-transform: uppercase;
      letter-spacing: 0.08em;
    }

    .item-tags {
      position: absolute;
      bottom: var(--space-3);
      left: var(--space-3);
      display: flex;
      gap: var(--space-1);
      flex-wrap: wrap;
    }

    .menu-item-body {
      padding: var(--space-4);
      display: flex;
      flex-direction: column;
      gap: var(--space-2);
      flex: 1;
    }

    .menu-item-top {
      display: flex;
      align-items: flex-start;
      justify-content: space-between;
      gap: var(--space-3);
    }

    .menu-item-name { font-size: 1rem; flex: 1; line-height: 1.3; }
    .menu-item-desc { font-size: 0.84rem; color: var(--color-text-muted); line-height: 1.6; flex: 1; }

    .menu-item-footer {
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-top: auto;
      padding-top: var(--space-3);
      border-top: 1px solid var(--color-border);
    }

    /* Cart Sticky */
    .cart-stick {
      position: fixed;
      bottom: var(--space-6);
      left: 50%;
      transform: translateX(-50%);
      z-index: 500;
      animation: fadeInUp 0.3s ease;
    }

    .cart-stick-inner {
      display: flex;
      align-items: center;
      gap: var(--space-4);
      background: var(--color-surface);
      border: 1px solid var(--color-accent);
      border-radius: var(--radius-full);
      padding: var(--space-3) var(--space-5);
      box-shadow: var(--shadow-accent);
      backdrop-filter: blur(12px);
      white-space: nowrap;
      font-size: 0.9rem;
    }

    /* Empty state */
    .empty-state {
      text-align: center;
      padding: var(--space-16) 0;
      color: var(--color-text-muted);
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: var(--space-4);
      font-size: 1.1rem;
    }

    @media (max-width: 600px) {
      .menu-grid { grid-template-columns: 1fr; }
      .category-tabs { display: grid; grid-template-columns: repeat(3,1fr); }
    }
  `]
})
export class MenuComponent implements OnInit {
  private menuService = inject(MenuService);
  private cartService = inject(CartService);

  categories: FilterCategory[] = ['All', 'Appetizers', 'Mains', 'Desserts', 'Drinks', 'Specials'];
  menuCategories: MenuCategory[] = ['Appetizers', 'Mains', 'Desserts', 'Drinks', 'Specials'];

  allItems        = signal<MenuItem[]>([]);
  filteredItems   = signal<MenuItem[]>([]);
  selectedCategory = signal<FilterCategory>('All');
  loading         = signal(true);
  searchQuery     = '';

  cartCount = this.cartService.totalItems;
  cartTotal = this.cartService.totalAmount;

  ngOnInit(): void {
    this.menuService.getMenu().subscribe(items => {
      this.allItems.set(items);
      this.filteredItems.set(items);
      this.loading.set(false);
    });
  }

  setCategory(cat: FilterCategory): void {
    this.selectedCategory.set(cat);
    this.searchQuery = '';
    this.applyFilter();
  }

  onSearch(query: string): void {
    this.selectedCategory.set('All');
    this.applyFilter();
  }

  clearSearch(): void {
    this.searchQuery = '';
    this.applyFilter();
  }

  private applyFilter(): void {
    let items = this.allItems();
    if (this.searchQuery.trim()) {
      const q = this.searchQuery.toLowerCase();
      items = items.filter(i =>
        i.name.toLowerCase().includes(q) ||
        i.description.toLowerCase().includes(q) ||
        i.category.toLowerCase().includes(q)
      );
    } else if (this.selectedCategory() !== 'All') {
      items = items.filter(i => i.category === this.selectedCategory());
    }
    this.filteredItems.set(items);
  }

  getItemsByCategory(cat: MenuCategory): MenuItem[] {
    return this.allItems().filter(i => i.category === cat);
  }

  addToCart(item: MenuItem): void {
    this.cartService.addItem(item);
  }

  getCategoryIcon(cat: string): string {
    const map: Record<string, string> = {
      All: 'food', Appetizers: 'category_appetizers', Mains: 'category_mains',
      Desserts: 'category_desserts', Drinks: 'category_drinks', Specials: 'category_specials'
    };
    return map[cat] ?? 'food';
  }
}
