import { NgModule } from '@angular/core';
import { ApplicationCreateComponent } from './application-create/application-create.component';
import { ApplicationModeratorComponent } from './application-moderator/application-moderator.component';
import { ApplicationModeratorListComponent } from './application-moderator-list/application-moderator-list.component';
import { ApplicationUserComponent } from './application-user/application-user.component';
import { ApplicationUserListComponent } from './application-user-list/application-user-list.component';
import { RouterModule } from '@angular/router';

import { applicationRoutes } from './application.routes';
import { SharedModule } from '../shared/shared.module';

@NgModule({
    imports: [
        SharedModule,
        RouterModule.forChild(applicationRoutes),
    ],
    exports: [],
    declarations: [
        ApplicationCreateComponent,
        ApplicationModeratorComponent,
        ApplicationModeratorListComponent,
        ApplicationUserComponent,
        ApplicationUserListComponent,
    ],
    providers: []
})
export class ApplicationModule { }
