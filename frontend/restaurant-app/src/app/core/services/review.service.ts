import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Review } from '../models/review.model';

const MOCK_REVIEWS: Review[] = [
  { id: 1, customerName: 'Alexandra M.', rating: 5, comment: 'An unforgettable culinary journey. The wagyu tenderloin was perfection — melt-in-your-mouth texture with truffle jus that\'s simply divine.', date: '2026-03-10', verified: true },
  { id: 2, customerName: 'James T.', rating: 5, comment: 'Noir & Orange has redefined fine dining for me. The atmosphere is intoxicating and every dish tells a story. The chocolate noir dessert alone is worth the visit.', date: '2026-03-05', verified: true },
  { id: 3, customerName: 'Sofia L.', rating: 4, comment: 'The charred octopus starter was exceptional. Service is attentive without being intrusive. Will be back for the tasting menu.', date: '2026-02-28', verified: true },
  { id: 4, customerName: 'Marcus K.', rating: 5, comment: 'The cocktail program here rivals the food. The Smoked Old Fashioned is a work of art. Beautifully crafted experience from start to finish.', date: '2026-02-20', verified: true },
];

@Injectable({ providedIn: 'root' })
export class ReviewService {
  private apiUrl = 'https://fakerestaurantapi.runasp.net/reviews';

  constructor(private http: HttpClient) {}

  getReviews(): Observable<Review[]> {
    return this.http.get<Review[]>(this.apiUrl).pipe(
      catchError(() => of(MOCK_REVIEWS))
    );
  }
}
