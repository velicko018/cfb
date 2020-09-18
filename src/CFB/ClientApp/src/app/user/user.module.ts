import { NgModule } from "@angular/core";

import { SharedModule } from "@/shared/shared.module";
import { UserRoutingModule } from "./user-routing.module";

import { BookingCreateComponent } from "./booking-create/booking-create.component";
import { MyBookingsComponent } from "./my-bookings/my-bookings.component";

@NgModule({
    imports: [
        SharedModule,
        UserRoutingModule
    ],
    declarations: [
        BookingCreateComponent,
        MyBookingsComponent
    ],
    exports: [
        BookingCreateComponent,
        MyBookingsComponent
    ]
})
export class UserModule { }
