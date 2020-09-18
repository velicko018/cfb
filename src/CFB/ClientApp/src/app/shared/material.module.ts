
import { NgModule } from "@angular/core";
import { CommonModule } from '@angular/common';
import {
    MatButtonModule, MatCardModule, MatDialogModule, MatInputModule, MatTableModule,
    MatToolbarModule, MatMenuModule, MatIconModule, MatProgressSpinnerModule, MatDatepickerModule, MatNativeDateModule, MatOptionModule, MatSelectModule, MatProgressBarModule, MatSidenavModule, MatPaginatorModule, MatSortModule, MatDividerModule, MatNavList, MatListModule, MatSnackBarModule, MatExpansionModule
} from '@angular/material';

@NgModule({
    imports: [
        CommonModule,
        MatToolbarModule,
        MatButtonModule,
        MatCardModule,
        MatInputModule,
        MatDialogModule,
        MatTableModule,
        MatMenuModule,
        MatIconModule,
        MatProgressSpinnerModule,
        MatProgressBarModule,
        MatDatepickerModule,
        MatNativeDateModule,
        MatSelectModule,
        MatOptionModule,
        MatSidenavModule,
        MatPaginatorModule,
        MatSortModule,
        MatDividerModule,
        MatListModule,
        MatSnackBarModule,
        MatExpansionModule
    ],
    exports: [
        CommonModule,
        MatToolbarModule,
        MatButtonModule,
        MatCardModule,
        MatInputModule,
        MatDialogModule,
        MatTableModule,
        MatMenuModule,
        MatIconModule,
        MatProgressSpinnerModule,
        MatProgressBarModule,
        MatDatepickerModule,
        MatNativeDateModule,
        MatSelectModule,
        MatOptionModule,
        MatSidenavModule,
        MatSortModule,
        MatPaginatorModule,
        MatDividerModule,
        MatListModule,
        MatSnackBarModule,
        MatExpansionModule
    ],
})
export class MaterialModule { }
