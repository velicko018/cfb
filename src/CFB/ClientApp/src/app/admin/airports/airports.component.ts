import { Component, AfterViewInit, ViewChild, OnInit, OnDestroy } from '@angular/core';
import { MatPaginator, MatSort, MatDialog, MatTableDataSource } from '@angular/material';
import { merge, of as observableOf, Subscription } from 'rxjs';
import { catchError, map, startWith, switchMap } from 'rxjs/operators';

import { Airport, PaginatedAirports } from '@/core/models';
import { AirportService } from '@/core/services';
import { ModalComponent } from '@/shared/components/modal/modal.component';
import { NotificationService } from '@/shared/services/notification.service';

import * as states from '../../../assets/states.json';

@Component({
    selector: 'app-airports-component',
    templateUrl: './airports.component.html',
    styles: ['./airports.component.css']
})
export class AirportsComponent implements AfterViewInit, OnInit, OnDestroy {
    displayedColumns: string[] = ['name', 'city', 'state', 'icao', 'iata', 'latitude', 'longitude', 'actions'];
    states: string[];
    dataSource: MatTableDataSource<Airport>;
    subscription: Subscription;
    resultsLength = 0;
    isLoadingResults = true;

    @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
    @ViewChild(MatSort, { static: false }) sort: MatSort;

    constructor(private airportService: AirportService,
        private notificationService: NotificationService,
        private dialog: MatDialog) {
        this.dataSource = new MatTableDataSource<Airport>([]);
        this.isLoadingResults = true;
        this.resultsLength = 0;
        this.subscription = new Subscription();
    }

    ngOnInit(): void {
        this.states = states;
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
    }

    ngAfterViewInit() {
        this.subscription.add(this.sort.sortChange.subscribe(() => {
            this.paginator.pageIndex = 0
        }));

        merge(this.sort.sortChange, this.paginator.page)
            .pipe(
                startWith({}),
                switchMap(() => {
                    this.isLoadingResults = true;

                    return this.airportService.getAirports(this.paginator.pageIndex, this.paginator.pageSize);
                }),
                map((data: PaginatedAirports) => {
                    this.isLoadingResults = false;
                    this.resultsLength = data.total;

                    return data;
                }),
                catchError(() => {
                    this.isLoadingResults = false;

                    return observableOf([]);
                })
            ).subscribe((paginatedAirports: PaginatedAirports) => {
                this.dataSource = new MatTableDataSource(paginatedAirports.airports);
            });
    }

    ngOnDestroy(): void {
        this.subscription.unsubscribe();
    }

    deleteAirport(id, name) {
        const dialogRef = this.dialog.open(ModalComponent, {
            data: { id, name }
        });
        dialogRef.componentInstance.yesClicked$
            .subscribe(result => {
                if (result === true) {
                    this.subscription.add(this.airportService.deleteAirport(id)
                        .subscribe(() => {
                            this.dataSource.data.splice(this.dataSource.data.findIndex(u => u.id == id), 1);
                            this.resultsLength--;
                            this.dataSource._updateChangeSubscription();

                            dialogRef.componentInstance.showLoader = false;

                            dialogRef.close();
                            this.notificationService
                                .success('Airport deleted successfully');
                        })
                    );
                }
            }, error => {
                console.log(error);
            });
    }
}
