import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, delay } from 'rxjs/operators';
import { Reservation, ReservationResponse } from '../models/reservation.model';

@Injectable({ providedIn: 'root' })
export class ReservationService {
  private apiUrl = 'https://fakerestaurantapi.runasp.net/reservations';

  constructor(private http: HttpClient) {}

  makeReservation(reservation: Reservation): Observable<ReservationResponse> {
    return this.http.post<ReservationResponse>(this.apiUrl, reservation).pipe(
      catchError(() => of({
        id: Math.floor(Math.random() * 10000),
        status: 'confirmed' as const,
        queuePosition: Math.floor(Math.random() * 5) + 1,
        message: 'Your reservation has been confirmed! We look forward to seeing you.'
      }).pipe(delay(1000)))
    );
  }

  getReservations(): Observable<Reservation[]> {
    return this.http.get<Reservation[]>(this.apiUrl).pipe(
      catchError(() => of([]))
    );
  }

  cancelReservation(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      catchError(() => of(undefined))
    );
  }
}
