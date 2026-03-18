import { Component, OnInit, signal, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MenuService } from '../../core/services/menu.service';
import { ReviewService } from '../../core/services/review.service';
import { CartService } from '../../core/services/cart.service';
import { MenuItem } from '../../core/models/menu-item.model';
import { Review } from '../../core/models/review.model';
import { IconComponent } from '../../shared/components/icon.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterLink, CommonModule, IconComponent],
  template: `
    <!-- Hero Section -->
    <section class="hero">
      <div class="hero-bg"></div>
      <div class="hero-overlay"></div>
      <div class="container hero-content">
        <div class="hero-text animate-fade-up">
          <span class="badge badge-accent">Est. 2019 · New York City</span>
          <h1 class="hero-title">
            A Symphony of<br>
            <span class="text-accent">NooR</span>
          </h1>
          <p class="hero-subtitle">
            Where culinary artistry meets obsidian elegance. Every dish is a narrative,
            every evening an unforgettable experience.
          </p>
          <div class="hero-actions">
            <a routerLink="/reservations" class="btn btn-primary btn-lg">
              Reserve Your Table
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M5 12h14M12 5l7 7-7 7"/></svg>
            </a>
            <a routerLink="/menu" class="btn btn-ghost btn-lg">Explore Menu</a>
          </div>
        </div>
        <div class="hero-stats">
          <div class="stat-card">
            <span class="stat-number">12+</span>
            <span class="stat-label">Years of Excellence</span>
          </div>
          <div class="stat-card">
            <span class="stat-number">48</span>
            <span class="stat-label">Curated Dishes</span>
          </div>
          <div class="stat-card">
            <span class="stat-number flex items-center justify-center gap-1"><app-icon name="star" fill="currentColor" strokeWidth="0"></app-icon> 4.9</span>
            <span class="stat-label">Guest Rating</span>
          </div>
        </div>
      </div>
      <div class="hero-scroll-indicator">
        <div class="scroll-line"></div>
        <span>Scroll to explore</span>
      </div>
    </section>

    <!-- Featured Items Section -->
    <section class="section">
      <div class="container">
        <div class="section-header">
          <span class="section-label">Hand-picked by our Chef</span>
          <h2 class="section-title">Tonight's Highlights</h2>
          <div class="divider"></div>
          <p class="section-subtitle">A curated selection of signature dishes that define the NooR experience.</p>
        </div>

        @if (loading()) {
          <div class="loading-state">
            <div class="spinner"></div>
          </div>
        } @else {
          <div class="featured-grid">
            @for (item of featuredItems(); track item.id) {
              <article class="featured-card card">
                <div class="featured-card-image">
                  <div class="image-placeholder">
                    <app-icon [name]="getCategoryIcon(item.category)" strokeWidth="1"></app-icon>
                  </div>
                  <div class="card-overlay">
                    <span class="badge badge-accent">{{ item.category }}</span>
                    @if (item.tags?.includes('chef-special')) {
                      <span class="chef-badge">Chef's Special</span>
                    }
                  </div>
                </div>
                <div class="featured-card-body">
                  <h3>{{ item.name }}</h3>
                  <p class="text-muted">{{ item.description }}</p>
                  <div class="card-footer">
                    <span class="price">\${{ item.price }}</span>
                    <button class="btn btn-primary btn-sm" (click)="addToCart(item)">
                      Add to Order
                    </button>
                  </div>
                </div>
              </article>
            }
          </div>

          <div class="text-center mt-10">
            <a routerLink="/menu" class="btn btn-outline btn-lg">View Full Menu</a>
          </div>
        }
      </div>
    </section>

    <!-- Experience Section -->
    <section class="experience-section section">
      <div class="container">
        <div class="experience-grid">
          <div class="experience-text">
            <span class="section-label">Our Philosophy</span>
            <h2 class="section-title text-left">The Art of<br><span class="text-accent">Fine Dining</span></h2>
            <div class="divider my-4 ml-0"></div>
            <p class="text-muted mb-4 leading-relaxed">
              At NooR, we believe that dining is more than sustenance — it's a complete sensory journey. Our kitchen brigade, led by Chef Marcus Voss, sources only the finest seasonal ingredients from trusted local farms and international purveyors.
            </p>
            <p class="text-muted mb-8 leading-relaxed">
              Every dish is composed with the care of a painter, the precision of a sculptor, and the heart of a storyteller.
            </p>
            <div class="experience-features">
              <div class="experience-feature">
                <app-icon name="leaf" class="feature-icon"></app-icon>
                <div>
                  <strong>Farm to Table</strong>
                  <p>Locally sourced, seasonal ingredients</p>
                </div>
              </div>
              <div class="experience-feature">
                <app-icon name="wine" class="feature-icon"></app-icon>
                <div>
                  <strong>Curated Wine Cellar</strong>
                  <p>500+ labels from 30+ countries</p>
                </div>
              </div>
              <div class="experience-feature">
                <app-icon name="award" class="feature-icon"></app-icon>
                <div>
                  <strong>Award-Winning</strong>
                  <p>Michelin recommended since 2022</p>
                </div>
              </div>
            </div>
          </div>
          <div class="experience-visual">
            <div class="visual-card">
              <div class="visual-inner">
                <app-icon name="diamondFill" class="visual-text"></app-icon>
                <h3>Tasting Menu</h3>
                <p>7-Course Experience</p>
                <p class="price text-xl mt-3">$185 pp</p>
              </div>
            </div>
            <div class="visual-tag">
              <span>Chef's Table Available</span>
              <a routerLink="/reservations" class="btn btn-primary btn-sm">Book Now</a>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Reviews Section -->
    <section class="section bg-surface">
      <div class="container">
        <div class="section-header">
          <span class="section-label">What Our Guests Say</span>
          <h2 class="section-title">Stories of Excellence</h2>
          <div class="divider"></div>
        </div>

        @if (reviews().length > 0) {
          <div class="reviews-grid">
            @for (review of reviews(); track review.id) {
              <div class="review-card card">
                <div class="review-header">
                  <div class="reviewer-avatar">{{ review.customerName[0] }}</div>
                  <div>
                    <strong>{{ review.customerName }}</strong>
                    <div class="stars">
                      @for (s of [1,2,3,4,5]; track s) {
                        @if (s <= review.rating) { <app-icon name="starFill" class="star text-accent"></app-icon> }
                        @else { <app-icon name="star" class="star text-muted"></app-icon> }
                      }
                    </div>
                  </div>
                  @if (review.verified) {
                    <span class="verified-badge"><app-icon name="check" strokeWidth="2.5" class="mr-1"></app-icon> Verified</span>
                  }
                </div>
                <p class="review-text">"{{ review.comment }}"</p>
                <span class="review-date text-muted">{{ review.date | date:'MMMM d, yyyy' }}</span>
              </div>
            }
          </div>
        }
      </div>
    </section>

    <!-- CTA Section -->
    <section class="cta-section">
      <div class="container cta-content">
        <h2 class="section-title">Ready for an Unforgettable Evening?</h2>
        <p class="section-subtitle">Reserve your table now and let us craft an experience tailored to you.</p>
        <div class="flex gap-4 justify-center flex-wrap mt-8">
          <a routerLink="/reservations" class="btn btn-primary btn-lg">Make a Reservation</a>
          <a routerLink="/menu" class="btn btn-outline btn-lg">Browse Our Menu</a>
        </div>
      </div>
    </section>
  `
})
export class HomeComponent implements OnInit {
  private menuService   = inject(MenuService);
  private reviewService = inject(ReviewService);
  private cartService   = inject(CartService);

  featuredItems = signal<MenuItem[]>([]);
  reviews       = signal<Review[]>([]);
  loading       = signal(true);

  ngOnInit(): void {
    this.menuService.getFeaturedItems().subscribe(items => {
      this.featuredItems.set(items.slice(0, 3));
      this.loading.set(false);
    });
    this.reviewService.getReviews().subscribe(r => this.reviews.set(r));
  }

  addToCart(item: MenuItem): void {
    this.cartService.addItem(item);
  }

  getCategoryIcon(category: string): string {
    const map: Record<string, string> = {
      Appetizers: 'category_appetizers', Mains: 'category_mains', Desserts: 'category_desserts',
      Drinks: 'category_drinks', Specials: 'category_specials'
    };
    return map[category] ?? 'food';
  }
}
