import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

import { AirportService } from '@/core/services';
import { NotificationService } from '@/shared/services/notification.service';

import * as states from '../../../assets/states.json';

@Component({
  selector: 'app-airport-form',
    templateUrl: './airport-form.component.html',
    styleUrls: ['./airport-form.component.css']
})
export class AirportFormComponent implements OnInit {
    airportForm: FormGroup;
    airportId: string;
    title: string;
    states: string[] = [];

    constructor(private router: Router,
        private route: ActivatedRoute,
        private airportService: AirportService,
        private notificationService: NotificationService) {
        this.states = (states as any).default
    }

    ngOnInit(): void {
        this.airportId = this.route.snapshot.params.id;
        this.title = this.airportId
            ? "Edit airport"
            : "Create a new airport";
        this.airportForm = new FormGroup({
            name: new FormControl('', Validators.required),
            city: new FormControl('', Validators.required),
            state: new FormControl('', Validators.required),
            latitude: new FormControl('', Validators.required),
            longitude: new FormControl('', Validators.required),
            iata: new FormControl('', Validators.required),
            icao: new FormControl('', Validators.required)
        });

        if (this.airportId) {
            this.airportService.getAirport(this.airportId).subscribe(data => this.airportForm.patchValue(data));
        }

    }

    submit() {
        if (this.airportForm.valid) {
            if (this.airportId) {
                this.airportService
                    .updateAirport(this.airportId, this.airportForm.value)
                    .subscribe((res: Response) => {
                        this.notificationService
                            .success('Airport updated successfully');
                        this.router.navigate(['/airports']);
                    });
            } else {
                this.airportService
                    .createAirport(this.airportForm.value)
                    .subscribe((res: Response) => {
                        this.notificationService
                            .success('Airport created successfully');
                        this.router.navigate(['/airports']);
                    });
            }
        }
    }

    cancel() {
        this.router.navigate(['/airports']);
    }

    hasError(key, value) {
        return this.airportForm
            .get(key)
            .hasError(value);
    }
}
