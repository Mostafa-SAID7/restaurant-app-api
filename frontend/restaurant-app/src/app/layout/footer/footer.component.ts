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
            <div class="footer-logo">
              <app-icon name="noorLogo" class="footer-logo-icon"></app-icon>
              <span>NooR</span>
            </div>
            <p class="footer-tagline">Where every meal is a masterpiece. Experience contemporary fine dining in a noir setting.</p>
            <div class="footer-socials">
              <a href="#" aria-label="Instagram" class="social-link">
                <app-icon name="instagram" strokeWidth="2"></app-icon>
              </a>
              <a href="#" aria-label="Facebook" class="social-link">
                <app-icon name="facebook" strokeWidth="2"></app-icon>
              </a>
              <a href="#" aria-label="Twitter" class="social-link">
                <app-icon name="twitter" strokeWidth="2"></app-icon>
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
              <li class="contact-detail">
                <app-icon name="mapPin" class="ui-icon"></app-icon>
                <span>18 West 29th Street<br>New York, NY 10001</span>
              </li>
              <li class="contact-detail">
                <app-icon name="phone" class="ui-icon"></app-icon>
                <a href="tel:+15556667">+(155) 556-667</a>
              </li>
              <li class="contact-detail">
                <app-icon name="envelope" class="ui-icon"></app-icon>
                <a href="mailto:reservations@noor.com">reservations&#64;noor.com</a>
              </li>
            </ul>
            <a routerLink="/reservations" class="btn btn-primary btn-sm footer-cta">
              Reserve a Table
            </a>
          </div>
        </div>

        <div class="footer-bottom">
          <p>© 2026 NooR. All rights reserved.</p>
          <div class="footer-legal">
            <a routerLink="/privacy">Privacy Policy</a>
            <a routerLink="/terms">Terms of Service</a>
          </div>
        </div>
      </div>
    </footer>
  `
})
export class FooterComponent {}
