import { Injectable } from '@angular/core';
import {
    HttpRequest,
    HttpHandler,
    HttpEvent,
    HttpInterceptor,
    HttpErrorResponse
} from '@angular/common/http';

import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError, switchMap, filter, take } from 'rxjs/operators';

import { AccountService, JwtTokenService } from '@/core/services';
import { Token } from '@/core/models';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {

    private tokenRefreshing: boolean = false;
    private refreshTokenSubject: BehaviorSubject<Token> = new BehaviorSubject<Token>(null);

    constructor(
        private accountService: AccountService,
        private jwtTokenService: JwtTokenService) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(this.addToken(request)).pipe(
            catchError(error => {
                if (error instanceof HttpErrorResponse && error.status === 401) {                 
                    return this.handle401Error(request, next);
                } else {
                    return throwError(error);
                }
            }));
    }

    private addToken(request: HttpRequest<any>) {
        let expiration = new Date(JSON.parse(localStorage.getItem('expiration')));

        if (expiration && +expiration - +new Date() > 0) {
            return request.clone({
                setHeaders: { 'Authorization': `Bearer ${localStorage.getItem('accessToken')}` }
            });
        }

        return request;
        
    }

    private handle401Error(request: HttpRequest<any>, next: HttpHandler) {
        if (this.tokenRefreshing) {
            return this.refreshTokenSubject
                .pipe(
                    filter(token => token != null),
                    take(1),
                    switchMap(jwt => {
                        return next.handle(this.addToken(request))
                    }));
        } else {
            this.tokenRefreshing = true;

            this.refreshTokenSubject.next(null);

            let token = new Token();
            token.accessToken = localStorage.getItem('accessToken');
            token.refreshToken = localStorage.getItem('refreshToken');
            token.expiration = new Date(JSON.parse(localStorage.getItem('expiration')));
            
            return this.accountService.refreshToken(token)
                .pipe(switchMap(result => {
                    this.tokenRefreshing = false;
                    this.refreshTokenSubject.next(result);                  
                    localStorage.setItem('accessToken', result.accessToken);
                    localStorage.setItem('refreshToken', result.refreshToken);
                    localStorage.setItem('expiration', JSON.stringify(result.expiration));
                    localStorage.setItem('role', this.jwtTokenService.getRole(result.accessToken));

                    return next.handle(this.addToken(request));
                }));
        }
    }    
}

