import { Component, AfterViewInit, ViewChild, OnInit } from '@angular/core';
import { MatPaginator, MatSort, ErrorStateMatcher } from '@angular/material';
import { FormGroup, FormControl, Validators, FormGroupDirective, NgForm } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

import { Airport } from '@/core/models';
import { AirportService, FlightService } from '@/core/services';
import { NotificationService } from '@/shared/services/notification.service';

import * as states from '../../../assets/states.json';

@Component({
    selector: 'app-flight-form',
    templateUrl: './flight-form.component.html',
    styleUrls: ['./flight-form.component.css']
})
export class FlightFormComponent implements OnInit {
    flightForm: FormGroup;
    sameValueMatcher = new SameValueEStateMatcher();
    flightId: string;
    title: string;

    constructor(private flightService: FlightService,
        private airportService: AirportService,
        private notificationService: NotificationService,
        private router: Router,
        private route: ActivatedRoute) {
        this.states = this.states = (states as any).default
    }

    private fromAirports: Airport[] = [];
    private toAirports: Airport[] = [];
    private fromAirportInfo: string;
    private toAirportInfo: string;
    private states: string[] = [];

    ngOnInit(): void {
        this.flightId = this.route.snapshot.params.id;
        this.title = this.flightId
            ? "Edit flight"
            : "Create a new flight";
        this.flightForm = new FormGroup({
            stateFrom: new FormControl(''),
            stateTo: new FormControl(''),
            from: new FormControl('', Validators.required),
            to: new FormControl('', Validators.required),
            departure: new FormControl('', Validators.required),
            duration: new FormControl('', Validators.compose([
                Validators.required,
                Validators.minLength(5),
                Validators.maxLength(5),
                Validators.pattern('([01]?[0-9]|2[0-3]):[0-5][0-9]')]))
        }, { validators: SameValueValidator });

        if (this.flightId) {
            this.flightService.getFlight(this.flightId)
                .subscribe(data => {
                    let originAirportInfo = data.originAirportInfo.split(' - ');
                    let destinationAirportInfo = data.destinationAirportInfo.split(' - ');
                    let test = {
                        stateFrom: originAirportInfo[1].match(/^([^(]*)/)[0].trim(),
                        stateTo: destinationAirportInfo[1].match(/^([^(]*)/)[0].trim(),
                        from: originAirportInfo[0],
                        to: destinationAirportInfo[0],
                        departure: new Date(data.departure),
                        duration: `${data.duration.slice(0, 5)}`
                    }

                    this.flightForm.patchValue(test);
                    this.flightForm.controls.from.disable();
                    this.flightForm.controls.to.disable();
                    this.flightForm.controls.stateFrom.disable();
                    this.flightForm.controls.stateTo.disable();
                });
        }
    }

    submit() {
        if (this.flightForm.valid) {
            if (this.flightId) {
                let data = {
                    departure: this.flightForm.controls.departure.value,
                    duration: this.flightForm.controls.duration.value
                };
                this.flightService.updateFlight(this.flightId, data)
                    .subscribe(res => {
                        this.notificationService.success('Flight updated successfully');
                        this.router.navigate(['/flights']);
                    });
            } else {
                let data = { originAirportInfo: this.fromAirportInfo, destinationAirportInfo: this.toAirportInfo, ...this.flightForm.value }

                this.flightService
                    .createFlight(data)
                    .subscribe((res: Response) => {
                        this.notificationService.success('Flight created successfully');
                        this.router.navigate(['/flights']);
                    });
            }
        }
    }

    cancel() {
        this.router.navigate(['/flights']);
    }

    hasError(key, value) {
        return this.flightForm
            .get(key)
            .hasError(value);
    }

    onStateChange(inputField, isFromSelect = true) {
        this.airportService.getAirportsByState(inputField.value)
            .subscribe(data => {
                if (isFromSelect) {
                    this.fromAirports = data;

                    if (data.length == 0) {
                        this.flightForm.get('from').reset();
                    }
                } else {

                    this.toAirports = data;

                    if (data.length == 0)
                        this.flightForm.get('to').reset();
                }
            });
    }


    onAirportChange(inputField, isFromSelect = true) {
        if (isFromSelect) {
            this.fromAirportInfo = inputField.source.triggerValue;
        } else {
            this.toAirportInfo = inputField.source.triggerValue;
        }
    }

}

export class SameValueEStateMatcher implements ErrorStateMatcher {
    isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
        return (control && control.parent.get('from').value != '' && control.parent.get('to').value !== '' && control.parent.get('from').value === control.parent.get('to').value)
    }
}

export function SameValueValidator(group: FormGroup) {
    const from = group.controls.from.value;
    const to = group.controls.to.value;

    return from === to ? { valuesAreSame: true } : null
}
