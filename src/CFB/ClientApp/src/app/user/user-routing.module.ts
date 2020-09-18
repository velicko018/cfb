import { NgModule } from "@angular/core";
import { RouterModule, Route } from "@angular/router";

import { Role } from "@/core/models/role";
import { AuthGuard } from "@/shared/guards/auth.guard";

import { MyBookingsComponent } from "./my-bookings/my-bookings.component";
import { BookingCreateComponent } from "./booking-create/booking-create.component";

const userRoutes: Route[] = [
    {
        path: 'bookings/create',
        component: BookingCreateComponent,
        canActivate: [AuthGuard],
        data: {
            roles: [Role.User]
        }
    },
    {
        path: 'my-bookings',
        component: MyBookingsComponent,
        canActivate: [AuthGuard],
        data: {
            roles: [Role.User]
        }
    }
]

@NgModule({
    imports: [
        RouterModule.forChild(userRoutes)
    ],
    exports: [
        RouterModule
    ],
    providers: [AuthGuard]
})
export class UserRoutingModule { }
