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
      <div class="container p-header-section">
        <span class="section-label">Our Culinary Offerings</span>
        <h1 class="section-title text-center title-lg">The Full Menu</h1>
        <div class="divider"></div>
        <p class="section-subtitle text-center mt-4 mx-auto mb-0">Every dish is composed with seasonal ingredients, artisan techniques, and a deep passion for flavour.</p>
      </div>
    </div>

    <div class="container mt-8 pb-20">

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
              <app-icon [name]="getCategoryIcon(cat)" class="mr-1"></app-icon> {{ cat }}
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
        <div class="flex justify-center py-16">
          <div class="spinner"></div>
        </div>
      } @else {

        <!-- Menu by Category or All -->
        @if (selectedCategory() === 'All' && !searchQuery) {
          @for (cat of menuCategories; track cat; let i = $index) {
            @if (getItemsByCategory(cat).length > 0) {
              <section class="menu-section">
                <div class="menu-section-header animate-fade-up" [style.animation-delay]="i * 0.1 + 's'">
                  <div class="menu-section-title">
                    <app-icon [name]="getCategoryIcon(cat)" class="menu-section-icon"></app-icon>
                    <span>{{ cat }}</span>
                  </div>
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
              <p><app-icon name="search" class="icon-lg"></app-icon> No dishes found for "{{ searchQuery }}"</p>
              <button class="btn btn-ghost" (click)="clearSearch()">Clear Search</button>
            </div>
          } @else {
            <div class="menu-grid mt-6">
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
            <span class="badge badge-accent text-xs">{{ item.category }}</span>
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
  `
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
