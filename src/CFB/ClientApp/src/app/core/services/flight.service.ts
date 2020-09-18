import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Journay, PaginatedFlights, Flight, UpdateFlight } from '@/core/models';

@Injectable({
    providedIn: 'root'
})
export class FlightService {
    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

    search(flight): Observable<Array<Journay>> {
        return this.http.post<Array<Journay>>(this.baseUrl + 'api/flights/search', flight);
    }

    getFlights(pageIndex, pageSize): Observable<PaginatedFlights> {
        return this.http.get<PaginatedFlights>(this.baseUrl + `api/flights?pageIndex=${pageIndex}&pageSize=${pageSize}`);
    }

    getFlight(flightId: string): Observable<Flight> {
        return this.http.get<Flight>(this.baseUrl + `api/flights/${flightId}`);
    }

    createFlight(flight: Flight) {
        return this.http.post(this.baseUrl + 'api/flights', flight);
    }

    deleteFlight(flightId: string) {
        return this.http.delete(this.baseUrl + `api/flights/${flightId}`);
    }

    updateFlight(flightId: string, updateFlight: UpdateFlight) {
        return this.http.put<UpdateFlight>(this.baseUrl + `api/flights/${flightId}`, updateFlight);
    }
}



