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
    file;
    base64image;

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
            icao: new FormControl('', Validators.required),
        });

        if (this.airportId) {
            this.airportService
                .getAirport(this.airportId)
                .subscribe(data => this.airportForm.patchValue(data));
        }

    }

    uploadFile(event) {
        this.file = event[0];
        const fileReader = new FileReader();
        fileReader.onload = () => {
            this.base64image = fileReader.result;
        };

        fileReader.readAsDataURL(this.file);
    }

    deleteFile() {
        this.file = null;
        this.base64image = null;
    }

    submit() {
        if (this.airportForm.valid) {
            const formData = new FormData();

            formData.append('image', this.file);
            formData.append('name', this.airportForm.value.name)
            formData.append('city', this.airportForm.value.city)
            formData.append('state', this.airportForm.value.state)
            formData.append('latitude', this.airportForm.value.latitude)
            formData.append('longitude', this.airportForm.value.longitude)
            formData.append('iata', this.airportForm.value.iata)
            formData.append('icao', this.airportForm.value.icao)

            if (this.airportId) {
                this.airportService
                    .updateAirport(this.airportId, formData)
                    .subscribe(() => {
                        this.notificationService
                            .success('Airport updated successfully');
                        this.router.navigate(['/airports']);
                    });
            } else {
                this.airportService
                    .createAirport(formData)
                    .subscribe(() => {
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
