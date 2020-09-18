import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from '@/core/services';

@Component({
    selector: 'app-nav-menu',
    templateUrl: './nav-menu.component.html',
    styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
    isExpanded = false;

    constructor(private accountService: AccountService,
        private router: Router) { }

    collapse() {
        this.isExpanded = false;
    }

    toggle() {
        this.isExpanded = !this.isExpanded;
    }

    get isAdmin() {
        return localStorage.getItem('role') == 'Admin';
    }

    get currentUser() {
        return localStorage.getItem('accessToken');
    }

    logout() {
        this.accountService
            .logout()
            .subscribe(res => {
                console.log(res);
                localStorage.clear();
                this.router.navigate(['/login']);
            }, err => console.log(err));
    }
}
