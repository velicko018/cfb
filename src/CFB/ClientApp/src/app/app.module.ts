import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppComponent } from './app.component';
import { NavMenuComponent } from '@/components/nav-menu/nav-menu.component';
import { HomeComponent } from '@/components/home/home.component';

import { CoreModule } from '@/core/core.module';
import { SharedModule } from '@/shared/shared.module';
import { AuthModule } from '@/auth/auth.module';
import { UserModule } from '@/user/user.module';
import { AdminModule } from '@/admin/admin.module';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        HomeComponent
    ],
    imports: [
        BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
        CoreModule,
        SharedModule,
        AuthModule,
        UserModule,
        AdminModule,
        RouterModule.forRoot([
            { path: '', component: HomeComponent, pathMatch: 'full' }
        ]),
        BrowserAnimationsModule
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
