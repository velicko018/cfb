import { OnInit, Component, OnDestroy } from "@angular/core";
import { FormGroup, FormControl, Validators, FormBuilder, FormArray } from "@angular/forms";
import { Router } from "@angular/router";
import { Subscription } from "rxjs";

import { BookingService } from "@/core/services";
import { Flight } from "@/core/models";

import { NotificationService } from "@/shared/services/notification.service";
import { DataService } from "@/shared/services/data.service";

@Component({    
    selector: 'app-booking-create-component',
    templateUrl: './booking-create.component.html',
    styleUrls: ['./booking-create.component.css']
})
export class BookingCreateComponent implements OnInit, OnDestroy  {
    bookingCreateForm: FormGroup;
    flightIds: string[];
    showForm: boolean;
    subscription: Subscription;

    constructor(private formBuilder: FormBuilder,
        private router: Router,
        private bookingService: BookingService,
        private dataService: DataService,
        private notificationService: NotificationService) {
    }

    ngOnInit(): void {
        this.subscription = new Subscription();
        this.subscription.add(this.dataService.currentData
            .subscribe((data: Flight[]) => {
                if (data) {
                    this.flightIds = Array.from(data, f => f.id);
                    this.showForm = true;
                    this.bookingCreateForm = this.formBuilder.group({
                        passangers: this.formBuilder.array([
                            new FormGroup({
                                firstName: new FormControl('', Validators.required),
                                lastName: new FormControl('', Validators.required),
                                phoneNumber: new FormControl('', Validators.required),
                                passportNumber: new FormControl('', Validators.required)
                            })
                        ])
                    });
                }
            })
        );
    }

    ngOnDestroy(): void {
        if (this.subscription) {
            this.subscription.unsubscribe();
        }
    }

    get passangers(): FormArray { 
        return this.bookingCreateForm.get('passangers') as FormArray;
    }

    addPassanger() {
        this.passangers.push(new FormGroup ({
            firstName: new FormControl('', Validators.required),
            lastName: new FormControl('', Validators.required),
            phoneNumber: new FormControl('', Validators.required),
            passportNumber: new FormControl('', Validators.required)
        }));
    }

    createBooking() {
        if (this.bookingCreateForm.valid) {
            this.subscription.add(this.bookingService.createBooking({ flightIds: this.flightIds, ... this.bookingCreateForm.value })
                .subscribe(_ => {
                    this.notificationService.success('Booking created successfully');
                    this.router.navigate(['/my-bookings']);
                })
            );
        }
    }

    hasError(key, value, index) {
        return this.passangers.controls[index]
            .get(key)
            .hasError(value);
    }
}


