import { Injectable, HostListener } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot, CanDeactivate } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PageRefreshGuard implements CanDeactivate<ComponentCanDeactivate> {
    canDeactivate(component: ComponentCanDeactivate,
        currentRoute: ActivatedRouteSnapshot,
        currentState: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
        return component.canDeactivate();

    }
}

export abstract class ComponentCanDeactivate {

    abstract canDeactivate(): Observable<boolean> | boolean;

    @HostListener('window:beforeunload', ['$event'])
    unloadNotification($event: any) {
        if (this.canDeactivate()) {
            $event.returnValue = true;
        }
        //this.canDeactivate().subscribe(value => {
        //    if (!value) {
        //        $event.returnValue = true;
        //    }
        //})
    }
}
