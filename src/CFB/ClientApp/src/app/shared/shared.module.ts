import { NgModule } from "@angular/core";
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from "@angular/forms";
import { FlexLayoutModule } from "@angular/flex-layout";

import { MaterialModule } from "./material.module";
import { ModalComponent } from "./components/modal/modal.component";
import { AuthGuard } from "./guards/auth.guard";
import { DragDropDirective } from "./directives/drag-drop.directive";

@NgModule({
    declarations: [
        ModalComponent,
        DragDropDirective
    ],
    entryComponents: [
        ModalComponent
    ],
    imports: [
        CommonModule,
        FlexLayoutModule,
        FormsModule,
        ReactiveFormsModule,
        MaterialModule
    ],
    exports: [
        CommonModule,
        FlexLayoutModule,
        FormsModule,
        ReactiveFormsModule,
        MaterialModule,
        DragDropDirective
    ],
    providers: [
        AuthGuard
    ]
})
export class SharedModule { }
