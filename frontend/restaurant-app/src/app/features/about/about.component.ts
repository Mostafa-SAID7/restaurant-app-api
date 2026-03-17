import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { IconComponent } from '../../shared/components/icon.component';

@Component({
  selector: 'app-about',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterLink, IconComponent],
  template: `
    <!-- Page Header -->
    <div class="page-header">
      <div class="page-header-bg"></div>
      <div class="container" style="position:relative;z-index:1;padding-top:calc(var(--header-h) + var(--space-12));padding-bottom:var(--space-12);text-align:center;">
        <span class="section-label">Our Story</span>
        <h1 class="section-title" style="font-size:clamp(2rem,5vw,3.5rem);">About Noir & Orange</h1>
        <div class="divider"></div>
        <p class="section-subtitle" style="margin:var(--space-4) auto 0;">A passion project turned into New York City's most talked-about culinary destination.</p>
      </div>
    </div>

    <!-- Story Section -->
    <section class="section">
      <div class="container">
        <div class="story-grid">
          <div class="story-visual">
            <div class="story-image-card">
              <div class="story-image-content">
                <app-icon name="diamondFill" class="story-icon"></app-icon>
                <h3>Since 2019</h3>
                <p>Crafting extraordinary dining experiences</p>
              </div>
            </div>
            <div class="story-badge-card card">
              <app-icon name="award" class="badge-icon" style="color:var(--color-accent);"></app-icon>
              <div>
                <strong>Michelin Recommended</strong>
                <p class="text-muted" style="font-size:0.83rem;">Since 2022</p>
              </div>
            </div>
          </div>

          <div class="story-content">
            <span class="section-label" style="text-align:left;display:block;">Our Beginning</span>
            <h2 style="font-size:2rem; margin-bottom:var(--space-5);">A Vision Born in Midnight Kitchens</h2>
            <p style="color:var(--color-text-muted);line-height:1.85;margin-bottom:var(--space-4);">
              Noir & Orange was born from a single obsession: creating food that tells a story. Founders Chef Marcus Voss and sommelier Lucia Chen envisioned a restaurant where the drama of fine dining met the warmth of genuine hospitality.
            </p>
            <p style="color:var(--color-text-muted);line-height:1.85;margin-bottom:var(--space-6);">
              Inspired by the chiaroscuro of classic jazz and the bold palette of late-night New York, they designed every detail of Noir & Orange to be an immersive experience — from the hand-crafted cocktail programme to the obsidian-tiled dining room.
            </p>
            <div class="story-stats">
              <div class="story-stat">
                <span class="stat-number text-accent">48+</span>
                <span class="text-muted" style="font-size:0.83rem;">Signature Dishes</span>
              </div>
              <div class="story-stat">
                <span class="stat-number text-accent">12</span>
                <span class="text-muted" style="font-size:0.83rem;">Expert Chefs</span>
              </div>
              <div class="story-stat">
                <span class="stat-number text-accent">500+</span>
                <span class="text-muted" style="font-size:0.83rem;">Wine Labels</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Team Section -->
    <section class="section" style="background: var(--color-surface);">
      <div class="container">
        <div class="section-header">
          <span class="section-label">The Talent Behind the Magic</span>
          <h2 class="section-title">Meet Our Team</h2>
          <div class="divider"></div>
        </div>
        <div class="team-grid">
          @for (member of team; track member.name) {
            <div class="team-card card">
              <app-icon [name]="member.icon" class="team-avatar" strokeWidth="1" style="color:var(--color-accent);"></app-icon>
              <h3 class="team-name">{{ member.name }}</h3>
              <div class="badge badge-accent" style="margin:var(--space-2) 0 var(--space-3);">{{ member.role }}</div>
              <p class="team-bio text-muted">{{ member.bio }}</p>
            </div>
          }
        </div>
      </div>
    </section>

    <!-- Values Section -->
    <section class="section">
      <div class="container">
        <div class="section-header">
          <span class="section-label">What We Stand For</span>
          <h2 class="section-title">Our Values</h2>
          <div class="divider"></div>
        </div>
        <div class="values-grid">
          @for (v of values; track v.title) {
            <div class="value-card card">
              <app-icon [name]="v.icon" class="value-icon" style="color:var(--color-accent);"></app-icon>
              <h3>{{ v.title }}</h3>
              <p class="text-muted">{{ v.description }}</p>
            </div>
          }
        </div>
      </div>
    </section>

    <!-- Contact & Map Section -->
    <section class="section" style="background: var(--color-surface);">
      <div class="container">
        <div class="section-header">
          <span class="section-label">Get In Touch</span>
          <h2 class="section-title">Contact Us</h2>
          <div class="divider"></div>
        </div>

        <div class="contact-layout">
          <!-- Contact Form -->
          <div class="contact-form-wrap card" style="padding:var(--space-8);">
            <h3 style="margin-bottom:var(--space-6);">Send a Message</h3>
            <form [formGroup]="contactForm" (ngSubmit)="sendMessage()" style="display:flex;flex-direction:column;gap:var(--space-4);">
              <div class="form-row-2">
                <div class="form-group">
                  <label class="form-label" for="cname">Your Name *</label>
                  <input id="cname" type="text" class="form-input" formControlName="name" placeholder="John Doe">
                </div>
                <div class="form-group">
                  <label class="form-label" for="cemail">Email Address *</label>
                  <input id="cemail" type="email" class="form-input" formControlName="email" placeholder="john@example.com">
                </div>
              </div>
              <div class="form-group">
                <label class="form-label" for="csubject">Subject *</label>
                <input id="csubject" type="text" class="form-input" formControlName="subject" placeholder="How can we help?">
              </div>
              <div class="form-group">
                <label class="form-label" for="cmessage">Message *</label>
                <textarea id="cmessage" class="form-input" formControlName="message" rows="5" placeholder="Tell us more..."></textarea>
              </div>

              @if (messageSent()) {
                <div class="success-msg"><app-icon name="check" strokeWidth="2" style="margin-right:4px;"></app-icon> Message sent! We'll get back to you within 24 hours.</div>
              }

              <button type="submit" class="btn btn-primary btn-lg" style="justify-content:center;" [disabled]="contactForm.invalid || sending()">
                @if (sending()) { <span class="spinner" style="width:18px;height:18px;border-width:2px;"></span> Sending... }
                @else { Send Message }
              </button>
            </form>
          </div>

          <!-- Contact Info -->
          <div class="contact-info">
            <div class="contact-detail-card card">
              <div class="contact-details">
                @for (detail of contactDetails; track detail.label) {
                  <div class="contact-detail">
                    <app-icon [name]="detail.icon" class="contact-icon" style="color:var(--color-accent);"></app-icon>
                    <div>
                      <span class="contact-label">{{ detail.label }}</span>
                      <p class="contact-value">{{ detail.value }}</p>
                    </div>
                  </div>
                }
              </div>
            </div>

            <div class="hours-card card">
              <h4 style="margin-bottom:var(--space-4);">Opening Hours</h4>
              <div class="hours-list">
                @for (h of hours; track h.days) {
                  <div class="hours-row-item">
                    <span class="text-muted">{{ h.days }}</span>
                    <span>{{ h.time }}</span>
                  </div>
                }
              </div>
            </div>

            <a routerLink="/reservations" class="btn btn-primary btn-lg" style="width:100%;justify-content:center;margin-top:var(--space-4);">
              Reserve a Table
            </a>
          </div>
        </div>
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
      background: radial-gradient(ellipse 60% 80% at 50% 100%, rgba(230,126,34,0.06) 0%, transparent 70%);
    }

    /* Story */
    .story-grid {
      display: grid;
      grid-template-columns: 1fr 1.4fr;
      gap: var(--space-12);
      align-items: center;
    }

    .story-visual {
      display: flex;
      flex-direction: column;
      gap: var(--space-4);
    }

    .story-image-card {
      background: linear-gradient(135deg, var(--color-surface-2), var(--color-surface-3));
      border: 1px solid var(--color-border);
      border-radius: var(--radius-xl);
      padding: var(--space-12);
      text-align: center;
      position: relative;
      overflow: hidden;

      &::before {
        content: '';
        position: absolute;
        top: 0;left:0;right:0;height:3px;
        background: linear-gradient(90deg, transparent, var(--color-accent), transparent);
      }
    }

    .story-icon {
      display: block;
      font-size: 2.5rem;
      color: var(--color-accent);
      margin-bottom: var(--space-4);
      animation: pulse-glow 3s ease-in-out infinite;
    }

    .story-badge-card {
      display: flex;
      align-items: center;
      gap: var(--space-4);
      padding: var(--space-4) var(--space-5);

      &:hover { transform: none; }
    }

    .badge-icon { font-size: 2rem; }

    .story-stats {
      display: flex;
      gap: var(--space-6);
    }

    .story-stat {
      display: flex;
      flex-direction: column;
      align-items: flex-start;
      gap: 2px;
    }

    .stat-number { font-size: 1.8rem; font-family: var(--font-serif); font-weight: 700; }

    /* Team */
    .team-grid {
      display: grid;
      grid-template-columns: repeat(3, 1fr);
      gap: var(--space-5);
    }

    .team-card {
      text-align: center;
      padding: var(--space-8) var(--space-6);
    }

    .team-avatar {
      font-size: 3rem;
      margin-bottom: var(--space-4);
    }

    .team-name { font-size: 1.1rem; }
    .team-bio { font-size: 0.86rem; line-height: 1.65; margin-top: var(--space-2); }

    /* Values */
    .values-grid {
      display: grid;
      grid-template-columns: repeat(3, 1fr);
      gap: var(--space-5);
    }

    .value-card {
      padding: var(--space-6);
      text-align: center;

      h3 { font-size: 1rem; margin: var(--space-3) 0 var(--space-2); }
      p { font-size: 0.86rem; line-height: 1.65; }
    }

    .value-icon { font-size: 2rem; }

    /* Contact */
    .contact-layout {
      display: grid;
      grid-template-columns: 1fr 380px;
      gap: var(--space-8);
      align-items: start;
    }

    .form-row-2 { display: grid; grid-template-columns: 1fr 1fr; gap: var(--space-4); }

    .contact-info {
      display: flex;
      flex-direction: column;
      gap: var(--space-4);
    }

    .contact-detail-card { padding: var(--space-6); }

    .contact-details {
      display: flex;
      flex-direction: column;
      gap: var(--space-4);
    }

    .contact-detail {
      display: flex;
      align-items: flex-start;
      gap: var(--space-4);
    }

    .contact-icon { font-size: 1.5rem; }
    .contact-label { font-size: 0.75rem; text-transform: uppercase; letter-spacing: 0.08em; color: var(--color-accent); display: block; margin-bottom: 3px; }
    .contact-value { font-size: 0.92rem; color: var(--color-text-muted); }

    .hours-card { padding: var(--space-5) var(--space-6); }

    .hours-list {
      display: flex;
      flex-direction: column;
      gap: var(--space-2);
    }

    .hours-row-item {
      display: flex;
      justify-content: space-between;
      font-size: 0.88rem;
      padding-bottom: var(--space-2);
      border-bottom: 1px solid var(--color-border);

      &:last-child { border-bottom: none; }
    }

    .success-msg {
      color: var(--color-success);
      font-size: 0.9rem;
      padding: var(--space-3) var(--space-4);
      background: rgba(39,174,96,0.08);
      border-radius: var(--radius-md);
      border: 1px solid rgba(39,174,96,0.2);
    }

    @media (max-width: 1024px) {
      .team-grid { grid-template-columns: repeat(2, 1fr); }
      .values-grid { grid-template-columns: repeat(2, 1fr); }
      .contact-layout { grid-template-columns: 1fr; }
    }

    @media (max-width: 768px) {
      .story-grid { grid-template-columns: 1fr; }
      .team-grid { grid-template-columns: 1fr; }
      .values-grid { grid-template-columns: 1fr; }
      .form-row-2 { grid-template-columns: 1fr; }
      .story-stats { flex-wrap: wrap; }
    }
  `]
})
export class AboutComponent {
  private fb = inject(FormBuilder);

  sending     = signal(false);
  messageSent = signal(false);

  contactForm = this.fb.group({
    name:    ['', Validators.required],
    email:   ['', [Validators.required, Validators.email]],
    subject: ['', Validators.required],
    message: ['', Validators.required],
  });

  team = [
    { name: 'Marcus Voss',    role: 'Executive Chef',    icon: 'chef', bio: 'Trained in Lyon and Tokyo, Chef Marcus brings two decades of Michelin-starred experience to every plate he creates at Noir & Orange.' },
    { name: 'Lucia Chen',     role: 'Head Sommelier',    icon: 'wine', bio: 'With a Masters in Viticulture from Bordeaux, Lucia curates our 500-label wine cellar and crafts the perfect pairing for every dish.' },
    { name: 'Sofia Renault',  role: 'Pastry Chef',       icon: 'category_desserts', bio: 'Sofia\'s dessert philosophy — precision meets poetry. Her chocolate noir has been featured in Bon Appétit and Food & Wine magazines.' },
  ];

  values = [
    { icon: 'leaf', title: 'Sustainability',      description: 'We source from certified organic farms and maintain a zero-food-waste kitchen policy.' },
    { icon: 'sparkles', title: 'Excellence',           description: 'Every dish that leaves our kitchen represents our relentless pursuit of perfection.' },
    { icon: 'handshake', title: 'Genuine Hospitality', description: 'We believe great food is made even better by genuine warmth and attentive service.' },
  ];

  contactDetails = [
    { icon: 'mapPin', label: 'Address', value: '18 West 29th Street, New York, NY 10001' },
    { icon: 'phone', label: 'Phone',   value: '+1 (555) NOIR-NYC' },
    { icon: 'envelope', label: 'Email',   value: 'reservations@noirorange.com' },
  ];

  hours = [
    { days: 'Mon – Thu', time: '6:00 pm – 11:00 pm' },
    { days: 'Fri – Sat', time: '6:00 pm – 1:00 am'  },
    { days: 'Sunday',    time: '6:00 pm – 10:00 pm' },
    { days: 'Lunch',     time: 'Fri – Sun from 12 pm' },
  ];

  sendMessage(): void {
    if (this.contactForm.invalid) { this.contactForm.markAllAsTouched(); return; }
    this.sending.set(true);
    setTimeout(() => {
      this.messageSent.set(true);
      this.sending.set(false);
      this.contactForm.reset();
    }, 1000);
  }
}
