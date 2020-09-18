import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
    constructor(private router: Router) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        let currentUser = localStorage.getItem('accessToken');       

        if (currentUser) {
            let role = localStorage.getItem('role');

            if (route.data.roles && !route.data.roles.includes(role)) {
                this.router.navigate(['/']);

                return false;
            }

            return true;
        }

        this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });

        return false;
    }
}
