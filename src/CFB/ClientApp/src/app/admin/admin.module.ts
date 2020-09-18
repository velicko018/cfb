import { NgModule } from "@angular/core";

import { SharedModule } from "@/shared/shared.module";
import { AdminRoutingModule } from "./admin-routing.module";

import { LogsComponent } from "./logs/logs.component";
import { BookingsComponent } from "./bookings/bookings.component";
import { AirportFormComponent } from "./airport-form/airport-form.component";
import { AirportsComponent } from "./airports/airports.component";
import { FlightsComponent } from "./flights/flights.component";
import { FlightFormComponent } from "./flight-form/flight-form.component";
import { UsersComponent } from "./users/users.component";
import { UserFormComponent } from "./user-form/user-form.component";

@NgModule({
    imports: [
        SharedModule,
        AdminRoutingModule
    ],
    declarations: [
        LogsComponent,
        BookingsComponent,
        AirportsComponent,
        AirportFormComponent,
        FlightsComponent,
        FlightFormComponent,
        UsersComponent,
        UserFormComponent
    ],
    exports: [
        LogsComponent,
        BookingsComponent,
        AirportsComponent,
        AirportFormComponent,
        FlightsComponent,
        FlightFormComponent,
        UsersComponent,
        UserFormComponent
    ]
})
export class AdminModule { }
