import { MatSnackBar } from "@angular/material";
import { Injectable } from "@angular/core";

@Injectable({ providedIn: 'root' })
export class NotificationService {
    constructor(private snackBar: MatSnackBar) { }

    success(message: string) {
        this.snackBar.open(message, 'X', {
            duration: 50000,
            verticalPosition: 'top',
            horizontalPosition: 'end',
            panelClass: ['success']
        });
    }

    error(message: string) {
        this.snackBar.open(message, 'X', {
            duration: 50000,
            verticalPosition: 'top',
            horizontalPosition: 'end',
            panelClass: ['error']
        });
    }
}
