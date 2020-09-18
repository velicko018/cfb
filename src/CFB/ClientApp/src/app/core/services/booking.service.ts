import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { PaginatedBookings, Booking } from '@/core/models';

@Injectable({
    providedIn: 'root'
})
export class BookingService {
    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

    getBookings(pageIndex: number, pageSize: number): Observable<PaginatedBookings> {
        return this.http.get<PaginatedBookings>(this.baseUrl + `api/bookings?pageIndex=${pageIndex}&pageSize=${pageSize}`);
    }

    createBooking(booking: Booking) {
        return this.http.post(this.baseUrl + 'api/bookings', booking);
    }

    getBookingsByUserId(userId: string): Observable<PaginatedBookings> {
        return this.http.get<PaginatedBookings>(this.baseUrl + `api/bookings/${userId}`);
    }
}

