import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

import { NotificationService } from '@/shared/services/notification.service';
import { UserService } from '@/core/services';

@Component({
    selector: 'app-user-form',
    templateUrl: './user-form.component.html',
    styleUrls: ['/user-form.component.css']
})
export class UserFormComponent implements OnInit {
    public userForm: FormGroup;
    private userId: string;

    constructor(private router: Router,
        private route: ActivatedRoute,
        private userService: UserService,
        private notificationService: NotificationService) {
    }

    ngOnInit() {
        this.userForm = new FormGroup({
            email: new FormControl('', Validators.compose([Validators.required, Validators.email])),
            firstName: new FormControl('', Validators.required),
            lastName: new FormControl('', Validators.required),
            address: new FormControl('', Validators.required),
            city: new FormControl('', Validators.required),
            zip: new FormControl('', Validators.required)
        });

        this.userId = this.route.snapshot.params.id;

        this.userService.getUser(this.userId)
            .subscribe(data => {
                this.userForm.patchValue(data);
            });
    }

    submit() {
        if (this.userForm.valid) {
            this.userService.updateUser(this.userId, this.userForm.value)
                .subscribe(data => {
                    this.notificationService.success('User updated successfully');
                    this.router.navigate(['/users']);
                });
        }
    }

    cancel() {
        this.router.navigate(['/users']);
    }


    hasError(key, value) {
        return this.userForm
            .get(key)
            .hasError(value);
    }
}


