import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, FormControl, Validators, FormGroupDirective, NgForm } from '@angular/forms';
import { ErrorStateMatcher } from '@angular/material';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';

import { AccountService } from '@/core/services';
import { NotificationService } from '@/shared/services/notification.service';

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrls: ['/register.component.css']
})
export class RegisterComponent implements OnInit, OnDestroy {
    registerForm: FormGroup;
    hide: boolean;
    passwordsMatcher = new RepeatPasswordEStateMatcher();
    private subscription: Subscription;

    constructor(private router: Router,
        private notificationService: NotificationService,
        private accountService: AccountService) {
        this.hide = true;
    }

    ngOnInit() {
        this.registerForm = new FormGroup({
            username: new FormControl('', Validators.required),
            email: new FormControl('', Validators.compose([Validators.required, Validators.email])),
            password: new FormControl('', Validators.compose([Validators.required, Validators.minLength(6)])),
            passwordConfirm: new FormControl(''),
            firstName: new FormControl('', Validators.required),
            lastName: new FormControl('', Validators.required),
            address: new FormControl('', Validators.required),
            city: new FormControl('', Validators.required),
            zip: new FormControl('', Validators.required)
        }, { validators: RepeatPasswordValidator });
    }

    ngOnDestroy(): void {
        if (this.subscription) {
            this.subscription.unsubscribe();
        }
    }

    register() {
        if (this.registerForm.valid) {
            this.subscription = this.accountService
                .register(this.registerForm.value)
                .subscribe(result => {
                    this.notificationService.success('User registered successfully.');
                    localStorage.setItem('accessToken', result.accessToken);
                    localStorage.setItem('refreshToken', result.refreshToken);
                    localStorage.setItem('expiration', JSON.stringify(result.expiration));
                    this.router.navigate(['/']);
                }, error => {
                    this.notificationService.error(`Something went wrong. ${error.error}`);
                });
        }
    }

    hasError(key, value) {
        return this.registerForm
            .get(key)
            .hasError(value);
    }
}

export class RepeatPasswordEStateMatcher implements ErrorStateMatcher {
    isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
        return (control && control.parent.get('password').value !== control.parent.get('passwordConfirm').value)
    }
}

export function RepeatPasswordValidator(group: FormGroup) {
    const password = group.controls.password.value;
    const passwordConfirmation = group.controls.passwordConfirm.value;

    return password === passwordConfirmation
        ? null
        : { passwordsNotEqual: true };
}


