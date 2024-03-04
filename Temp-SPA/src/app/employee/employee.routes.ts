import { Routes } from "@angular/router";
import { EmployeeListComponent } from "./employee-list/employee-list.component";
import { EmployeeEditComponent } from "./employee-edit/employee-edit.component";
import { EmployeeEditResolver } from "../core/resolvers/employee/employee-edit.resolver";
import { EmployeeCreateComponent } from "./employee-create/employee-create.component";
import { EmployeeAssignRoleComponent } from "./employee-assign-role/employee-assign-role.component";
import { EmployeeListResolver } from "../core/resolvers/employee/employee-list.resolver";

export const employeeRoutes: Routes = [
    { 
        path: '', 
        component: EmployeeListComponent, 
        resolve: {employees: EmployeeListResolver}
    },
    { 
        path: ':id/edit', 
        component: EmployeeEditComponent,
        resolve: { employee: EmployeeEditResolver }
    },
    {
        path: 'create',
        component: EmployeeCreateComponent
    },
    {
        path: ':id/assign-role',
        component: EmployeeAssignRoleComponent,
        resolve:  {employee: EmployeeEditResolver}
    }
];