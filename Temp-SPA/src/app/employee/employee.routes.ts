import { Routes } from "@angular/router";
import { AuthGuard } from "../core/guards/auth.guard";
import { EmployeeListComponent } from "./employee-list/employee-list.component";
import { EmployeeListResolver } from "../core/resolvers/employee/employee-list.resolver";
import { EmployeeEditComponent } from "./employee-edit/employee-edit.component";
import { EmployeeEditResolver } from "../core/resolvers/employee/employee-edit.resolver";
import { OrganizationListResolver } from "../core/resolvers/organization/organization-list.resolver";
import { EmployeeCreateComponent } from "./employee-create/employee-create.component";
import { EmployeeAssignRoleComponent } from "./employee-assign-role/employee-assign-role.component";

export const employeeRoutes: Routes = [
    {
        path: 'employees',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            { path: '', component: EmployeeListComponent, resolve: {employees: EmployeeListResolver}},
            { 
                path: ':id/edit', 
                component: EmployeeEditComponent,
                resolve: { employee: EmployeeEditResolver, organizations: OrganizationListResolver }
            },
            {
                path: 'create',
                component: EmployeeCreateComponent,
                resolve: { organizations: OrganizationListResolver }
            },
            {
                path: ':id/assign-role',
                component: EmployeeAssignRoleComponent,
                resolve:  {employee: EmployeeEditResolver}
            }
        ]
    }
];