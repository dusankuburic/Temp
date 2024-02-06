import { NgModule } from '@angular/core';
import { EmployeeAssignRoleComponent } from './employee-assign-role/employee-assign-role.component';
import { EmployeeCreateComponent } from './employee-create/employee-create.component';
import { EmployeeEditComponent } from './employee-edit/employee-edit.component';
import { EmployeeListComponent } from './employee-list/employee-list.component';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from '../app-routing.module';
import { PaginationModule } from 'ngx-bootstrap/pagination';


@NgModule({
    imports: [
        CommonModule,
        ReactiveFormsModule,
        AppRoutingModule,
        PaginationModule.forRoot(),
        FormsModule,
    ],
    exports: [],
    declarations: [
        EmployeeAssignRoleComponent,
        EmployeeCreateComponent,
        EmployeeEditComponent,
        EmployeeListComponent,
    ],
})
export class EmployeeModule { }
