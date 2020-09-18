import { Component, AfterViewInit, ViewChild } from '@angular/core';
import { MatPaginator, MatSort, MatTableDataSource } from '@angular/material';
import { merge, Observable, of as observableOf } from 'rxjs';
import { catchError, map, startWith, switchMap } from 'rxjs/operators';

import { Log, PaginatedLogs } from '@/core/models';
import { LogService } from '@/core/services';

@Component({
  selector: 'app-logs-component',
    templateUrl: './logs.component.html',
    styles: ['./logs.component.css']
})
export class LogsComponent implements AfterViewInit {
    displayedColumns: string[] = ['id', 'message', 'createdAt'];
    dataSource: MatTableDataSource<Log>;

    resultsLength = 0;
    isLoadingResults = true;
    isRateLimitReached = false;

    @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
    @ViewChild(MatSort, { static: false }) sort: MatSort;

    constructor(private logService: LogService) { }

    ngAfterViewInit() {
        this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

        merge(this.sort.sortChange, this.paginator.page)
            .pipe(
                startWith({}),
                switchMap(() => {
                    this.isLoadingResults = true;

                    return this.logService.getLogs(this.paginator.pageIndex, this.paginator.pageSize);
                }),
                map(data => {
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
            ).subscribe((data: PaginatedLogs) => this.dataSource = new MatTableDataSource<Log>(data.logs));
    }
}
