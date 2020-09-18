import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { FlightService, AirportService } from '@/core/services';
import { Airport, Journay } from '@/core/models';
import { DataService } from '@/shared/services/data.service';

import * as states from '../../../assets/states.json';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
    searchForm: FormGroup;
    showProgressBar: boolean;
    private states: string[];
    private fromAirports: Airport[] = [];
    private toAirports: Airport[] = [];
    private journays: Journay[];

    constructor(private router: Router,
        private flightService: FlightService,
        private airportService: AirportService,
        private dataService: DataService) {
        this.showProgressBar = false;
        this.states = (states as any).default;
    }

    ngOnInit(): void {
        this.searchForm = new FormGroup({
            stateFrom: new FormControl(''),
            stateTo: new FormControl(''),
            from: new FormControl('', Validators.required),
            to: new FormControl('', Validators.required),
            departure: new FormControl('', Validators.required),
        });
    }

    get isAdmin() {
        return localStorage.getItem('role') == 'Admin';
    }

    hasError(key, value) {
        return this.searchForm
            .get(key)
            .hasError(value);
    }

    onStateChange(inputField, isFromSelect = true) {
        this.airportService.getAirportsByState(inputField.value)
            .subscribe(data => {
                if (isFromSelect) {
                    this.fromAirports = data;

                    if (data.length == 0) {
                        this.searchForm.get('from').reset();
                    }
                } else {
                    this.toAirports = data;

                    if (data.length == 0)
                        this.searchForm.get('to').reset();
                }
            });
    }
    bookJournay(journay: Journay) {
        this.dataService.changeData(journay);
        this.router.navigate(['/bookings/create']);
    }

    public search() {
        if (this.searchForm.valid) {
            this.showProgressBar = true;
            this.flightService.search(this.searchForm.value)
                .subscribe(result => {
                    this.showProgressBar = false;
                    this.journays = result;
                });
        }
    }
}
