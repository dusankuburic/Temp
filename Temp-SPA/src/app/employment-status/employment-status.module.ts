import { NgModule } from '@angular/core';
import { EmploymentStatusCreateComponent } from './employment-status-create/employment-status-create.component';
import { EmploymentStatusEditComponent } from './employment-status-edit/employment-status-edit.component';
import { EmploymentStatusListComponent } from './employment-status-list/employment-status-list.component';
import { RouterModule } from '@angular/router';
import { employmentStatusRoutes } from './employment-status.routes';
import { SharedModule } from '../shared/shared.module';
import { EmploymentStatusListResolver } from '../core/resolvers/employment-status/employment-status-list.resolver';
import { EmploymentStatusEditResolver } from '../core/resolvers/employment-status/employment-status-edit.resolver';
import { EmploymentStatusValidators } from './employment-status-validators';

@NgModule({
    imports: [
        SharedModule,
        RouterModule.forChild(employmentStatusRoutes)
    ],
    exports: [],
    declarations: [
        EmploymentStatusCreateComponent,
        EmploymentStatusEditComponent,
        EmploymentStatusListComponent,
    ],
    providers: [
        EmploymentStatusValidators,
        EmploymentStatusListResolver,
        EmploymentStatusEditResolver,
    ]
})
export class EmploymentStatusModule { }
