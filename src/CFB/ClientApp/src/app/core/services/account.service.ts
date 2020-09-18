import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Token, Register, Login } from '@/core/models';

@Injectable({
    providedIn: 'root'
})
export class AccountService {
    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

    login(login: Login): Observable<Token> {
        return this.http.post<Token>(this.baseUrl + 'api/accounts/login', login);
    }

    register(register: Register): Observable<Token> {
        return this.http.post<Token>(this.baseUrl + 'api/accounts/register', register); 
    }

    logout() {
        return this.http.post(this.baseUrl + 'api/accounts/logout', {});
    }

    refreshToken(token: Token): Observable<Token> {
        const headers = new HttpHeaders();

        return this.http.post<Token>(this.baseUrl + 'api/accounts/refresh-token', token, { headers });
    }

    currentUser() {
        return this.http.get(this.baseUrl + 'api/accounts/current-user');
    }
}
