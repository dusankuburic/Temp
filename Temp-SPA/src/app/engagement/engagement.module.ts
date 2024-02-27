import { NgModule } from '@angular/core';
import { EngagementCreateComponent } from './engagement-create/engagement-create.component';
import { EngagementUserListComponent } from './engagement-user-list/engagement-user-list.component';
import { EngagementWithEmployeeListComponent } from './engagement-with-employee-list/engagement-with-employee-list.component';
import { EngagementWithoutEmployeeListComponent } from './engagement-without-employee-list/engagement-without-employee-list.component';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { RouterModule } from '@angular/router';
import { engagementRoutes } from './engagement.routes';
import { SharedModule } from '../shared/shared.module';
import { EngagementWithEmployeeResolver } from '../core/resolvers/engagement/engagement-with-employee-list.resolver';
import { EngagementWithoutEmployeeResolver } from '../core/resolvers/engagement/engagement-without-employee-list.resolver';
import { EngagementUserListResolver } from '../core/resolvers/engagement/engagement-user-list.resolver';
import { EngagementCreateResolver } from '../core/resolvers/engagement/engagement-create.resolver';

@NgModule({
    imports: [
        SharedModule,
        BsDatepickerModule,
        RouterModule.forChild(engagementRoutes)
    ],
    exports: [],
    declarations: [
        EngagementCreateComponent,
        EngagementUserListComponent,
        EngagementWithEmployeeListComponent,
        EngagementWithoutEmployeeListComponent,
    ],
    providers: [
        EngagementWithoutEmployeeResolver,
        EngagementCreateResolver,
        EngagementWithEmployeeResolver,
        EngagementUserListResolver,
    ]
})
export class EngagementModule { }
