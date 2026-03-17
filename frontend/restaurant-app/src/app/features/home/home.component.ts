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
            <span class="text-accent">Noir & Flavour</span>
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
            <span class="stat-number" style="display:flex;align-items:center;justify-content:center;gap:4px;"><app-icon name="star" fill="currentColor" strokeWidth="0"></app-icon> 4.9</span>
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
          <p class="section-subtitle">A curated selection of signature dishes that define the Noir & Orange experience.</p>
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

          <div style="text-align:center; margin-top: var(--space-10);">
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
            <h2 class="section-title" style="text-align:left;">The Art of<br><span class="text-accent">Fine Dining</span></h2>
            <div class="divider" style="margin:var(--space-4) 0;"></div>
            <p style="color:var(--color-text-muted); line-height:1.8; margin-bottom:var(--space-4);">
              At Noir & Orange, we believe that dining is more than sustenance — it's a complete sensory journey. Our kitchen brigade, led by Chef Marcus Voss, sources only the finest seasonal ingredients from trusted local farms and international purveyors.
            </p>
            <p style="color:var(--color-text-muted); line-height:1.8; margin-bottom:var(--space-8);">
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
                <p class="price" style="font-size:1.4rem; margin-top:var(--space-3);">$185 pp</p>
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
    <section class="section" style="background: var(--color-surface);">
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
                        @if (s <= review.rating) { <app-icon name="starFill" class="star" style="color:var(--color-accent);"></app-icon> }
                        @else { <app-icon name="star" class="star" style="color:var(--color-text-muted);"></app-icon> }
                      }
                    </div>
                  </div>
                  @if (review.verified) {
                    <span class="verified-badge"><app-icon name="check" strokeWidth="2.5" style="margin-right:2px;"></app-icon> Verified</span>
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
        <div style="display:flex; gap:var(--space-4); justify-content:center; flex-wrap:wrap; margin-top:var(--space-8);">
          <a routerLink="/reservations" class="btn btn-primary btn-lg">Make a Reservation</a>
          <a routerLink="/menu" class="btn btn-outline btn-lg">Browse Our Menu</a>
        </div>
      </div>
    </section>
  `,
  styles: [`
    /* Hero */
    .hero {
      position: relative;
      min-height: 100vh;
      display: flex;
      flex-direction: column;
      justify-content: center;
      overflow: hidden;
    }

    .hero-bg {
      position: absolute;
      inset: 0;
      background:
        radial-gradient(ellipse 80% 60% at 60% 40%, rgba(230,126,34,0.08) 0%, transparent 60%),
        radial-gradient(ellipse 60% 40% at 20% 70%, rgba(230,126,34,0.04) 0%, transparent 50%);
      background-color: var(--color-bg);
    }

    .hero-overlay {
      position: absolute;
      inset: 0;
      background: radial-gradient(circle at 50% 50%, transparent 0%, rgba(0,0,0,0.3) 100%);
    }

    .hero-content {
      position: relative;
      padding-top: calc(var(--header-h) + var(--space-12));
      padding-bottom: var(--space-20);
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: var(--space-10);
      align-items: end;
    }

    .hero-text { max-width: 560px; }

    .badge { margin-bottom: var(--space-5); display: inline-block; }

    .hero-title {
      font-size: clamp(2.5rem, 5vw, 4rem);
      font-family: var(--font-serif);
      line-height: 1.15;
      margin-bottom: var(--space-5);
    }

    .hero-subtitle {
      font-size: 1.05rem;
      color: var(--color-text-muted);
      line-height: 1.75;
      margin-bottom: var(--space-8);
      max-width: 480px;
    }

    .hero-actions {
      display: flex;
      gap: var(--space-4);
      flex-wrap: wrap;
    }

    .hero-stats {
      display: flex;
      flex-direction: column;
      gap: var(--space-4);
      align-items: flex-end;
    }

    .stat-card {
      background: rgba(255,255,255,0.04);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-xl);
      padding: var(--space-4) var(--space-6);
      text-align: center;
      backdrop-filter: blur(10px);
      min-width: 140px;
      transition: border-color var(--transition-base);

      &:hover { border-color: rgba(230,126,34,0.4); }
    }

    .stat-number {
      display: block;
      font-size: 1.6rem;
      font-family: var(--font-serif);
      font-weight: 700;
      color: var(--color-accent);
    }

    .stat-label {
      font-size: 0.8rem;
      color: var(--color-text-muted);
      text-transform: uppercase;
      letter-spacing: 0.08em;
    }

    .hero-scroll-indicator {
      position: absolute;
      bottom: var(--space-8);
      left: 50%;
      transform: translateX(-50%);
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: var(--space-2);
      color: var(--color-text-dim);
      font-size: 0.75rem;
      letter-spacing: 0.1em;
      text-transform: uppercase;
    }

    .scroll-line {
      width: 1px;
      height: 48px;
      background: linear-gradient(to bottom, var(--color-accent), transparent);
      animation: fadeIn 2s ease infinite alternate;
    }

    /* Featured Grid */
    .featured-grid {
      display: grid;
      grid-template-columns: repeat(3, 1fr);
      gap: var(--space-6);
    }

    .featured-card-image {
      position: relative;
      height: 200px;
    }

    .image-placeholder {
      width: 100%;
      height: 100%;
      background: linear-gradient(135deg, var(--color-surface-2), var(--color-surface-3));
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 3rem;
    }

    .card-overlay {
      position: absolute;
      top: var(--space-3);
      left: var(--space-3);
      right: var(--space-3);
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
    }

    .chef-badge {
      background: rgba(230,126,34,0.9);
      color: white;
      font-size: 0.7rem;
      font-weight: 700;
      padding: 2px 8px;
      border-radius: var(--radius-full);
      letter-spacing: 0.05em;
      text-transform: uppercase;
    }

    .featured-card-body {
      padding: var(--space-5);

      h3 {
        font-size: 1.15rem;
        margin-bottom: var(--space-2);
      }

      p { font-size:0.88rem; line-height:1.6; margin-bottom:var(--space-4); }
    }

    .card-footer {
      display: flex;
      align-items: center;
      justify-content: space-between;
    }

    /* Experience */
    .experience-section { background: var(--color-bg); }

    .experience-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: var(--space-16);
      align-items: center;
    }

    .experience-features {
      display: flex;
      flex-direction: column;
      gap: var(--space-4);
    }

    .experience-feature {
      display: flex;
      align-items: flex-start;
      gap: var(--space-4);

      .feature-icon { font-size: 1.5rem; }

      strong { display: block; margin-bottom: var(--space-1); }
      p { font-size: 0.85rem; color: var(--color-text-muted); }
    }

    .experience-visual {
      display: flex;
      flex-direction: column;
      gap: var(--space-4);
    }

    .visual-card {
      background: linear-gradient(135deg, var(--color-surface-2) 0%, var(--color-surface-3) 100%);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-xl);
      padding: var(--space-12);
      text-align: center;
      position: relative;
      overflow: hidden;

      &::before {
        content: '';
        position: absolute;
        top: 0;
        left: 0;
        right: 0;
        height: 3px;
        background: linear-gradient(90deg, transparent, var(--color-accent), transparent);
      }
    }

    .visual-text {
      display: block;
      font-size: 2.5rem;
      color: var(--color-accent);
      margin-bottom: var(--space-4);
      animation: pulse-glow 3s ease-in-out infinite;
    }

    .visual-inner h3 { font-size: 1.5rem; margin-bottom: var(--space-2); }
    .visual-inner p { color: var(--color-text-muted); }

    .visual-tag {
      display: flex;
      align-items: center;
      justify-content: space-between;
      background: var(--color-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-lg);
      padding: var(--space-4) var(--space-5);
      font-size: 0.9rem;
      color: var(--color-text-muted);
    }

    /* Reviews */
    .reviews-grid {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: var(--space-5);
    }

    .review-card {
      padding: var(--space-6);

      &:hover { transform: translateY(-2px); }
    }

    .review-header {
      display: flex;
      align-items: center;
      gap: var(--space-3);
      margin-bottom: var(--space-4);
    }

    .reviewer-avatar {
      width: 44px;
      height: 44px;
      border-radius: 50%;
      background: var(--color-accent-muted);
      border: 2px solid var(--color-accent);
      color: var(--color-accent);
      display: flex;
      align-items: center;
      justify-content: center;
      font-weight: 700;
      flex-shrink: 0;
    }

    .verified-badge {
      margin-left: auto;
      font-size: 0.75rem;
      color: var(--color-success);
    }

    .review-text {
      font-size: 0.92rem;
      color: var(--color-text-muted);
      line-height: 1.7;
      font-style: italic;
      margin-bottom: var(--space-3);
    }

    .review-date { font-size: 0.8rem; }

    /* CTA */
    .cta-section {
      padding: var(--space-20) 0;
      background: linear-gradient(135deg, var(--color-surface) 0%, rgba(230,126,34,0.06) 100%);
      border-top: 1px solid var(--color-border);
      text-align: center;
    }

    .loading-state {
      display: flex;
      justify-content: center;
      padding: var(--space-16) 0;
    }

    @media (max-width: 1024px) {
      .featured-grid { grid-template-columns: repeat(2,1fr); }
      .reviews-grid { grid-template-columns: 1fr; }
    }

    @media (max-width: 768px) {
      .hero-content { grid-template-columns: 1fr; }
      .hero-stats { flex-direction: row; align-items: flex-start; flex-wrap: wrap; }
      .stat-card { min-width: 0; flex: 1; }
      .featured-grid { grid-template-columns: 1fr; }
      .experience-grid { grid-template-columns: 1fr; }
    }
  `]
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
