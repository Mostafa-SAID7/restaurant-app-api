import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./features/home/home.component').then(m => m.HomeComponent),
    title: 'Noir & Orange — Fine Dining'
  },
  {
    path: 'menu',
    loadComponent: () =>
      import('./features/menu/menu.component').then(m => m.MenuComponent),
    title: 'Our Menu — Noir & Orange'
  },
  {
    path: 'reservations',
    loadComponent: () =>
      import('./features/reservations/reservations.component').then(m => m.ReservationsComponent),
    title: 'Reserve a Table — Noir & Orange'
  },
  {
    path: 'checkout',
    loadComponent: () =>
      import('./features/checkout/checkout.component').then(m => m.CheckoutComponent),
    title: 'Order Online — Noir & Orange'
  },
  {
    path: 'about',
    loadComponent: () =>
      import('./features/about/about.component').then(m => m.AboutComponent),
    title: 'About & Contact — Noir & Orange'
  },
  {
    path: 'privacy',
    loadComponent: () =>
      import('./features/privacy/privacy.component').then(m => m.PrivacyComponent),
    title: 'Privacy Policy — Noir & Orange'
  },
  {
    path: 'terms',
    loadComponent: () =>
      import('./features/terms/terms.component').then(m => m.TermsComponent),
    title: 'Terms of Service — Noir & Orange'
  },
  {
    path: '**',
    redirectTo: '',
    pathMatch: 'full'
  }
];
