import { NgModule } from '@angular/core';
import { ApplicationCreateComponent } from './application-create/application-create.component';
import { ApplicationModeratorComponent } from './application-moderator/application-moderator.component';
import { ApplicationModeratorListComponent } from './application-moderator-list/application-moderator-list.component';
import { ApplicationUserComponent } from './application-user/application-user.component';
import { ApplicationUserListComponent } from './application-user-list/application-user-list.component';
import { RouterModule } from '@angular/router';

import { applicationRoutes } from './application.routes';
import { ApplicationCreateResolver } from '../core/resolvers/application/application-create.resolver';
import { ApplicationModeratorListResolver } from '../core/resolvers/application/application-moderator-list.resolver';
import { ApplicationUserListResolver } from '../core/resolvers/application/application-user-list.resolver';
import { ApplicationUserResolver } from '../core/resolvers/application/application-user.resolver';
import { ApplicationModeratorResolver } from '../core/resolvers/application/application-moderator.resolver';
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
    providers: [
        ApplicationCreateResolver,
        ApplicationUserListResolver,
        ApplicationUserResolver,
        ApplicationModeratorListResolver,
        ApplicationModeratorResolver,
    ]
})
export class ApplicationModule { }
