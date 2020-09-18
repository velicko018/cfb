import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { PaginatedUsers, User } from '@/core/models';

@Injectable({
    providedIn: 'root'
})
export class UserService {
    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

    getUsers(pageIndex, pageSize): Observable<PaginatedUsers> {
        return this.http.get<PaginatedUsers>(this.baseUrl + `api/users?pageIndex=${pageIndex}&pageSize=${pageSize}`);
    }

    getUser(userId: string): Observable<User> {
        return this.http.get<User>(this.baseUrl + 'api/users/' + userId);
    }

    deleteUser(userId: string) {
        return this.http.delete(this.baseUrl + 'api/users/' + userId);
    }

    updateUser(userId: string, user: User) {
        return this.http.put(this.baseUrl + 'api/users/' + userId, user);
    }
}


