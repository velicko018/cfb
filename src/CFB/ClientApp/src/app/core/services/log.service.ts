import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { PaginatedLogs } from '@/core/models';

@Injectable({
    providedIn: 'root'
})
export class LogService {
    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

    getLogs(pageIndex, pageSize): Observable<PaginatedLogs> {
        return this.http.get<PaginatedLogs>(this.baseUrl + `api/logs?pageIndex=${pageIndex}&pageSize=${pageSize}`);
    }
}


