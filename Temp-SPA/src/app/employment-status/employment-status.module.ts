import { NgModule } from '@angular/core';
import { EmploymentStatusCreateComponent } from './employment-status-create/employment-status-create.component';
import { EmploymentStatusEditComponent } from './employment-status-edit/employment-status-edit.component';
import { EmploymentStatusListComponent } from './employment-status-list/employment-status-list.component';
import { RouterModule } from '@angular/router';
import { employmentStatusRoutes } from './employment-status.routes';
import { SharedModule } from '../shared/shared.module';
import { EmploymentStatusEditModalComponent } from './employment-status-edit-modal/employment-status-edit-modal.component';
import { EmploymentStatusCreateModalComponent } from './employment-status-create-modal/employment-status-create-modal.component';

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
        EmploymentStatusCreateModalComponent,
        EmploymentStatusEditModalComponent
    ],
    providers: []
})
export class EmploymentStatusModule { }
