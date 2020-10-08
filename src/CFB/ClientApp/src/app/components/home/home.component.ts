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
    showFirstStop: boolean;
    showSecondStop: boolean;
    showCustomStop: boolean;
    private states: string[];
    private airports: Map<string, Airport[]>;
    private journays: Journay[];

    constructor(private router: Router,
        private flightService: FlightService,
        private airportService: AirportService,
        private dataService: DataService) {
        this.showProgressBar = false;
        this.showFirstStop = false;
        this.showCustomStop = false;
        this.showSecondStop = false;
        this.states = (states as any).default;
        this.airports = new Map<string, Airport[]>()
    }

    ngOnInit(): void {
        this.searchForm = new FormGroup({
            stateFrom: new FormControl(''),
            stateTo: new FormControl(''),
            from: new FormControl('', Validators.required),
            to: new FormControl('', Validators.required),
            departure: new FormControl('', Validators.required),            
            numberOfStops: new FormControl(-1),
            firstStop: new FormControl(),
            secondStop: new FormControl(),
            customStop: new FormControl()
        });
    }

    get isAdmin() {
        return localStorage.getItem('role') === 'Admin';
    }

    hasError(key, value) {
        return this.searchForm
            .get(key)
            .hasError(value);
    }

    onStateChange(inputField, airportFieldType) {
        this.airportService.getAirportsByState(inputField.value)
            .subscribe(data => {
                this.airports[airportFieldType] = data;

                if (data.length === 0) {
                    this.searchForm.get(airportFieldType).reset();
                }
            });
    }

    onStopChange(inputField) {
        if (inputField.value === 1) {
            this.showFirstStop = true;
            this.showSecondStop = false;
            this.showCustomStop = false;
            this.airports['secondStop'] = [];
            this.searchForm.get('secondStop').reset();
            this.searchForm.get('customStop').reset();
        } else if (inputField.value === 2) {
            this.showFirstStop = false;
            this.showSecondStop = true;
            this.showCustomStop = false;
            this.searchForm.get('customStop').reset();
        } else if (inputField.value === 3) {
            this.showFirstStop = false;
            this.showSecondStop = false;
            this.showCustomStop = true;
            this.searchForm.get('firstStop').reset();
            this.searchForm.get('secondStop').reset();
        } else {
            this.showFirstStop = false;
            this.showSecondStop = false;
            this.showCustomStop = false;
            this.airports['firstStop'] = [];
            this.airports['secondStop'] = [];
            this.searchForm.get('firstStop').reset();
            this.searchForm.get('secondStop').reset();
            this.searchForm.get('customStop').reset();
        }
    }

    bookJournay(journay: Journay) {
        this.dataService.changeData(journay);
        this.router.navigate(['/bookings/create']);
    }

    search() {
        if (this.searchForm.valid) {
            this.showProgressBar = true;
            const customStopValue = this.searchForm.get('customStop').value;
            const searchFormValue = this.searchForm.value;

            if (customStopValue >= 0 && customStopValue !== null) {
                searchFormValue.numberOfStops = customStopValue;
            }

            this.flightService.search(searchFormValue)
                .subscribe(result => {
                    this.showProgressBar = false;
                    this.journays = result;
                });
        }
    }
}
