import { Component, AfterViewInit, ViewChild, OnInit } from '@angular/core';
import { MatPaginator, MatSort, MatDialog, MatTableDataSource } from '@angular/material';
import { merge, of as observableOf } from 'rxjs';
import { catchError, map, startWith, switchMap } from 'rxjs/operators';

import { Booking, PaginatedBookings } from '@/core/models';
import { BookingService, JwtTokenService } from '@/core/services';
import { ModalComponent } from '@/shared/components/modal/modal.component';

@Component({
  selector: 'app-my-bookings-component',
    templateUrl: './my-bookings.component.html',
    styles: ['./my-bookings.component.css']
})
export class MyBookingsComponent implements AfterViewInit, OnInit {
    displayedColumns: string[] = ['id', 'numberOfSeats', 'numberOfStops'];
    dataSource: MatTableDataSource<Booking>;

    resultsLength = 0;
    isLoadingResults = true;
    isRateLimitReached = false;

    @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
    @ViewChild(MatSort, { static: false }) sort: MatSort;
    userId: any;

    constructor(private bookingService: BookingService, public dialog: MatDialog, private jwtTokenService: JwtTokenService) {
        this.dataSource = new MatTableDataSource<Booking>([]);
        this.isLoadingResults = true;
        this.isRateLimitReached = false;
        this.resultsLength = 0;
    }

    ngOnInit() {
        this.userId = this.jwtTokenService.getUserId(localStorage.getItem('accessToken'));
    }

    ngAfterViewInit() {
        this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);
        merge(this.sort.sortChange, this.paginator.page)
            .pipe(
                startWith({}),
                switchMap(() => {
                    this.isLoadingResults = true;

                    return this.bookingService.getBookingsByUserId(this.userId);
                }),
                map((data: PaginatedBookings) => {
                    this.isLoadingResults = false;
                    this.isRateLimitReached = false;
                    this.resultsLength = data.total;

                    return data;
                }),
                catchError(() => {
                    this.isLoadingResults = false;
                    this.isRateLimitReached = true;

                    return observableOf([]);
                })
            ).subscribe((data: PaginatedBookings) => this.dataSource = new MatTableDataSource(data.bookings));
    }

    deleteBooking(id, name) {
        const dialogRef = this.dialog.open(ModalComponent, {
            data: { id, name }
        });
        dialogRef.componentInstance.yesClicked$
            .subscribe(result => {
                if (result == true) {
                    /*this.bookingService.deleteBooking(id)
                        .subscribe((res: Response) => {
                            this.dataSource.data.splice(this.dataSource.findIndex(u => u.id == id), 1);
                            this.resultsLength--;
                            this.dataSource._updateChangeSubscription();

                            dialogRef.componentInstance.showLoader = false;

                            dialogRef.close();
                        });*/
                }
            });
    }
}
