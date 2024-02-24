import { NgModule } from '@angular/core';
import { EmploymentStatusCreateComponent } from './employment-status-create/employment-status-create.component';
import { EmploymentStatusEditComponent } from './employment-status-edit/employment-status-edit.component';
import { EmploymentStatusListComponent } from './employment-status-list/employment-status-list.component';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from '../app-routing.module';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { RouterModule } from '@angular/router';
import { employmentStatusRoutes } from './employment-status.routes';

@NgModule({
    imports: [
        CommonModule,
        ReactiveFormsModule,
        AppRoutingModule,
        PaginationModule.forRoot(),
        FontAwesomeModule,
        FormsModule,
        RouterModule.forChild(employmentStatusRoutes)
    ],
    exports: [],
    declarations: [
        EmploymentStatusCreateComponent,
        EmploymentStatusEditComponent,
        EmploymentStatusListComponent,
    ],
})
export class EmploymentStatusModule { }
