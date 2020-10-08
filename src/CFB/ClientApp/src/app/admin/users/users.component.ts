import { Component, AfterViewInit, ViewChild } from '@angular/core';
import { MatPaginator, MatSort, MatDialog, MatTableDataSource, MatSnackBar } from '@angular/material';
import { merge, of as observableOf } from 'rxjs';
import { catchError, map, startWith, switchMap } from 'rxjs/operators';
import { ModalComponent } from '@/shared/components/modal/modal.component';
import { User, PaginatedUsers } from '@/core/models';
import { UserService } from '@/core/services';

@Component({
    selector: 'app-users-component',
    templateUrl: './users.component.html',
    styles: ['./users.component.css']
})
export class UsersComponent implements AfterViewInit {
    displayedColumns: string[] = ['id', 'email', 'username', 'firstName', 'lastName', 'address', 'city', 'zip', 'actions'];
    data: User[] = [];
    dataSource: MatTableDataSource<User>;
    resultsLength = 0;
    isLoadingResults = true;
    isRateLimitReached = false;

    @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
    @ViewChild(MatSort, { static: false }) sort: MatSort;

    constructor(private userService: UserService,
        public dialog: MatDialog,
        private snackBar: MatSnackBar) { }

    ngAfterViewInit() {
        this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

        merge(this.sort.sortChange, this.paginator.page)
            .pipe(
                startWith({}),
                switchMap(() => {
                    this.isLoadingResults = true;

                    return this.userService.getUsers(this.paginator.pageIndex, this.paginator.pageSize);
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
            ).subscribe((data: PaginatedUsers) => this.dataSource = new MatTableDataSource<User>(data.users));
    }

    deleteUser(id, username) {
        const dialogRef = this.dialog.open(ModalComponent, {
            data: { title: `Delete user ${username}` }

        });
        dialogRef.componentInstance.yesClicked$
            .subscribe(result => {
                if (result === true) {
                    this.userService.deleteUser(id)
                        .subscribe(() => {
                            this.dataSource.data.splice(this.data.findIndex(u => u.id == id), 1);
                            this.resultsLength--;
                            this.dataSource._updateChangeSubscription();

                            dialogRef.componentInstance.showLoader = false;

                            dialogRef.close();
                            this.snackBar.open('User deleted successfully', null, {
                                duration: 5000,
                                verticalPosition: 'top',
                                horizontalPosition: 'end',
                                panelClass: ['success'],
                            })
                        });
                }
            });
    }
}
