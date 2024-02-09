import { NgModule } from '@angular/core';
import { ApplicationCreateComponent } from './application-create/application-create.component';
import { ApplicationModeratorComponent } from './application-moderator/application-moderator.component';
import { ApplicationModeratorListComponent } from './application-moderator-list/application-moderator-list.component';
import { ApplicationUserComponent } from './application-user/application-user.component';
import { ApplicationUserListComponent } from './application-user-list/application-user-list.component';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from '../app-routing.module';


@NgModule({
    imports: [
        CommonModule,
        ReactiveFormsModule,
        AppRoutingModule,
    ],
    exports: [],
    declarations: [
        ApplicationCreateComponent,
        ApplicationModeratorComponent,
        ApplicationModeratorListComponent,
        ApplicationUserComponent,
        ApplicationUserListComponent,
    ],
})
export class ApplicationModule { }
