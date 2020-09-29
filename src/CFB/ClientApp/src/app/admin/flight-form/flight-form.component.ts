import { Component, OnInit } from '@angular/core';
import { ErrorStateMatcher } from '@angular/material';
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
            duration: new FormControl(1),
            departureTime: new FormControl(1)
        }, { validators: SameValueValidator });

        if (this.flightId) {
            this.flightService.getFlight(this.flightId)
                .subscribe(data => {
                    let originAirportInfo = data.originAirportInfo.split(' - ');
                    let destinationAirportInfo = data.destinationAirportInfo.split(' - ');
                    let departureDateTime = new Date(data.departure);
                    let departureTimeString = `${departureDateTime.getUTCHours()}:${departureDateTime.getUTCMinutes()}`;
                    let flightData = {
                        stateFrom: originAirportInfo[1].match(/^([^(]*)/)[0].trim(),
                        stateTo: destinationAirportInfo[1].match(/^([^(]*)/)[0].trim(),
                        from: originAirportInfo[0],
                        to: destinationAirportInfo[0],
                        departure: departureDateTime,
                        departureTime: this.stringToTime(departureTimeString),
                        duration: this.stringToTime(data.duration)
                    };
                    
                    this.flightForm.patchValue(flightData);
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
                let departureTime = this.formatTime(this.flightForm.controls.departureTime.value)
                    .toString()
                    .split(':');
                let departure = new Date(this.flightForm.controls.departure.value);
                let duration = this.formatTime(this.flightForm.controls.duration.value);

                departure.setUTCHours(Number.parseInt(departureTime[0]), Number.parseInt(departureTime[1]));

                let flightData = { departure, duration };

                this.flightService.updateFlight(this.flightId, flightData)
                    .subscribe(res => {
                        this.notificationService.success('Flight updated successfully');
                        this.router.navigate(['/flights']);
                    });
            } else {
                let flightData = {
                    originAirportInfo: this.fromAirportInfo,
                    destinationAirportInfo: this.toAirportInfo,
                    ...this.flightForm.value
                }

                let departureTime = this.formatTime(this.flightForm.controls.departureTime.value)
                    .toString()
                    .split(':');
                let departureDateTime = new Date(flightData.departure);

                departureDateTime.setUTCHours(Number.parseInt(departureTime[0]), Number.parseInt(departureTime[1]));

                flightData.departure = departureDateTime;
                flightData.duration = this.formatTime(flightData.duration);

                this.flightService
                    .createFlight(flightData)
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

                    if (data.length === 0) {
                        this.flightForm.get('from').reset();
                    }
                } else {

                    this.toAirports = data;

                    if (data.length === 0) {
                        this.flightForm.get('to').reset();
                    }
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

    formatTime(value: number | null) {
        const decimalPart = +value.toString().replace(/^[^\.]+/, '0');
        let mmPart = (decimalPart * 60).toString();

        if (mmPart.length === 1) {
            mmPart = mmPart + '0';
        }

        let hhPart = value.toFixed(2).split('.').pop();

        if (hhPart.length === 1) {
            hhPart = '0' + hhPart;
        }

        return hhPart + ':' + mmPart;
    }

    stringToTime(value) {
        const splitedTime = value.slice(0, 5).split(':');
        const hours = +splitedTime[0];
        const minutes = 1 / (60 / +splitedTime[1]);

        return hours + minutes;
    }
}

export class SameValueEStateMatcher implements ErrorStateMatcher {
    isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
        return (control &&
            control.parent.get('from').value !== '' &&
            control.parent.get('to').value !== '' &&
            control.parent.get('from').value === control.parent.get('to').value);
    }
}

export function SameValueValidator(group: FormGroup) {
    const from = group.controls.from.value;
    const to = group.controls.to.value;

    return from === to
        ? { valuesAreSame: true }
        : null
}
