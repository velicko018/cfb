import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';

import { AccountService, JwtTokenService } from '@/core/services';
import { NotificationService } from '@/shared/services/notification.service';

@Component({
    selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit, OnDestroy {
    public loginForm: FormGroup;
    public hide: boolean;
    private subscription: Subscription;

    constructor(private accountService: AccountService,
        private jwtTokenService: JwtTokenService,
        private notificationService: NotificationService,
        private router: Router) {
        this.hide = true;
    }

    ngOnInit() {
        this.loginForm = new FormGroup({
            email: new FormControl('', Validators.compose([Validators.required, Validators.email])),
            password: new FormControl('', [Validators.required])
        });
    }

    ngOnDestroy(): void {
        if (this.subscription) {
            this.subscription.unsubscribe();
        }
    }

    get email() {
        return this.loginForm.get('email');
    }

    get password() {
        return this.loginForm.get('password');
    }

    login() {
        if (this.loginForm.valid) {
            this.subscription = this.accountService
                .login(this.loginForm.value)
                .subscribe(result => {
                    localStorage.setItem('role', this.jwtTokenService.getRole(result.accessToken));
                    localStorage.setItem('accessToken', result.accessToken);
                    localStorage.setItem('refreshToken', result.refreshToken);
                    localStorage.setItem('expiration', JSON.stringify(result.expiration));
                    this.router.navigate(['/']);
                }, error => {
                        this.notificationService.error(`Something went wrong. ${error.error}`);
                });
        }
    }
}


