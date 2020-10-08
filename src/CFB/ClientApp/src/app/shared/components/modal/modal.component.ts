import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material';
import { BehaviorSubject } from 'rxjs';

import { DialogData } from '@/shared/models/dialog-data';

@Component({
  selector: 'app-modal',
  templateUrl: './modal.component.html'
})
export class ModalComponent {
    showLoader: boolean;
    yesClicked$: BehaviorSubject<boolean>;

    constructor(@Inject(MAT_DIALOG_DATA) public dialogData: DialogData) {
        this.showLoader = false;
        this.yesClicked$ = new BehaviorSubject<boolean>(false);
    }

    onYesClick() {
        this.showLoader = true;
        this.yesClicked$.next(true);
    }
}

