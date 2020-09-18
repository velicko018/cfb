import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";

import { AuthGuard } from "@/shared/guards/auth.guard";
import { Role } from "@/core/models/role";

import { LogsComponent } from "./logs/logs.component";
import { BookingsComponent } from "./bookings/bookings.component";
import { AirportFormComponent } from "./airport-form/airport-form.component";
import { AirportsComponent } from "./airports/airports.component";
import { FlightFormComponent } from "./flight-form/flight-form.component";
import { FlightsComponent } from "./flights/flights.component";
import { UsersComponent } from "./users/users.component";
import { UserFormComponent } from "./user-form/user-form.component";

const adminRoutes = [
    {
        path: 'logs',
        component: LogsComponent,
        canActivate: [AuthGuard],
        data: {
            roles: [Role.Admin]
        },
    },
    {
        path: 'airports',
        canActivate: [AuthGuard],
        data: {
            roles: [Role.Admin]
        },
        children: [
            { path: '', component: AirportsComponent },
            { path: 'create', component: AirportFormComponent },
            { path: ':id/edit', component: AirportFormComponent }
        ]
    },
    {
        path: 'flights',
        canActivate: [AuthGuard],
        data: {
            roles: [Role.Admin]
        },
        children: [
            { path: '', component: FlightsComponent },
            { path: 'create', component: FlightFormComponent },
            { path: ':id/edit', component: FlightFormComponent }
        ]
    },
    {
        path: 'bookings',
        component: BookingsComponent,
        canActivate: [AuthGuard],
        data: {
            roles: [Role.Admin]
        }
    },
    {
        path: 'users',
        canActivate: [AuthGuard],
        data: {
            roles: [Role.Admin]
        },
        children: [
            { path: '', component: UsersComponent },
            { path: ':id/edit', component: UserFormComponent }
        ]
    }
];

@NgModule({
    imports: [
        RouterModule.forChild(adminRoutes)
    ],
    exports: [
        RouterModule
    ]
})
export class AdminRoutingModule { }
