
import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Airport, PaginatedAirports } from '@/core/models';

@Injectable({
    providedIn: 'root'
})
export class AirportService {

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

    getAirports(pageIndex, pageSize): Observable<PaginatedAirports> {
        return this.http.get<PaginatedAirports>(this.baseUrl + `api/airports?pageIndex=${pageIndex}&pageSize=${pageSize}`);
    }

    getAirport(airportId): Observable<Airport> {
        return this.http.get<Airport>(this.baseUrl + 'api/airports/' + airportId)
    }

    getAirportsByState(state: string): Observable<Airport[]> {
        return this.http.get<Airport[]>(this.baseUrl + `api/airports/search?state=${state}`);
    }

    createAirport(airport: Airport) {
        return this.http.post(this.baseUrl + 'api/airports', airport);
    }

    updateAirport(airportId: string, airport: Airport) {
        return this.http.put(this.baseUrl + 'api/airports/' + airportId, airport);
    }

    deleteAirport(airportId: string) {
        return this.http.delete(this.baseUrl + 'api/airports/' + airportId);
    }

}
