import { Component, AfterViewInit, ViewChild } from '@angular/core';
import { MatPaginator, MatSort, MatDialog, MatTableDataSource } from '@angular/material';
import { merge, of as observableOf } from 'rxjs';
import { catchError, map, startWith, switchMap } from 'rxjs/operators';
import { Router } from '@angular/router';

import { Flight, PaginatedFlights } from '@/core/models';
import { FlightService } from '@/core/services';
import { ModalComponent } from '@/shared/components/modal/modal.component';
import { NotificationService } from '@/shared/services/notification.service';

@Component({
    selector: 'app-flights-component',
    templateUrl: './flights.component.html',
    styles: ['./flights.component.css']
})
export class FlightsComponent implements AfterViewInit {
    displayedColumns: string[] = ['from', 'to', 'departure', 'duration', 'actions'];
    data: Flight[] = [];
    dataSource: MatTableDataSource<Flight>;
    resultsLength = 0;
    isLoadingResults = true;

    @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
    @ViewChild(MatSort, { static: false }) sort: MatSort;

    constructor(private flightService: FlightService, private router: Router, private dialog: MatDialog, private notificationService: NotificationService) { }

    ngAfterViewInit() {
        this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

        merge(this.sort.sortChange, this.paginator.page)
            .pipe(
                startWith({}),
                switchMap(() => {
                    this.isLoadingResults = true;

                    return this.flightService.getFlights(this.paginator.pageIndex, this.paginator.pageSize);
                }),
                map((data: PaginatedFlights) => {
                    this.isLoadingResults = false;
                    this.resultsLength = data.total;

                    return data;
                }),
                catchError(() => {
                    this.isLoadingResults = false;

                    return observableOf([]);
                })
            ).subscribe((data: PaginatedFlights) => this.dataSource = new MatTableDataSource(data.flights));
    }

    deleteFlight(flightId: string, fromAirportInfo: string, toAirportInfo: string) {
        let fromICAO = fromAirportInfo.match(/\((.*)\)/).pop();
        let toICAO = toAirportInfo.match(/\((.*)\)/).pop();
        const dialogRef = this.dialog.open(ModalComponent, {
            data: { title: `Delete flight ${fromICAO} -> ${toICAO} ` }
        });

        dialogRef.componentInstance.yesClicked$
            .subscribe(result => {
                if (result == true) {
                    this.flightService.deleteFlight(flightId)
                        .subscribe((res: Response) => {
                            this.dataSource.data.splice(this.dataSource.data.findIndex(u => u.id == flightId), 1);
                            this.resultsLength--;
                            this.dataSource._updateChangeSubscription();

                            dialogRef.componentInstance.showLoader = false;

                            dialogRef.close();
                            this.notificationService.success('Flight deleted successfully');
                        });
                }
            });
    }
}
