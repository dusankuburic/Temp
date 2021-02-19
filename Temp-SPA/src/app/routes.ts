import { Routes } from '@angular/router';
import { EmployeeAssignRoleComponent } from './employee/employee-assign-role/employee-assign-role.component';
import { EmployeeCreateComponent } from './employee/employee-create/employee-create.component';
import { EmployeeEditComponent } from './employee/employee-edit/employee-edit.component';
import { EmployeeListComponent } from './employee/employee-list/employee-list.component';
import { HomeComponent } from './home/home.component';
import { UsersComponent } from './users/users.component';
import { WorkplaceCreateComponent } from './workplace/workplace-create/workplace-create.component';
import { WorkplaceListComponent } from './workplace/workplace-list/workplace-list.component';
import { AuthGuard } from './_guards/auth.guard';
import { UserGuard } from './_guards/user.guard';
import { EmployeeEditResolver } from './_resolvers/employee-edit.resolver';
import { EmployeeListResolver } from './_resolvers/employee-list.resolver';
import { WorkplaceListResolver } from './_resolvers/workplace-list.resolver';

export const appRoutes: Routes = [
    {path: '', component: HomeComponent},
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            {path: 'employees', component: EmployeeListComponent,
                resolve: {employees: EmployeeListResolver}},
            {path: 'employees/:id', component: EmployeeEditComponent,
                resolve: {employee: EmployeeEditResolver}},
            {path: 'employee/create', component: EmployeeCreateComponent},
            {path: 'employee/assign-role/:id', component: EmployeeAssignRoleComponent,
                resolve: {employee: EmployeeEditResolver}},
            {path: 'workplaces', component: WorkplaceListComponent,
                resolve: {workplaces: WorkplaceListResolver}},
                // Edit Workplace
            {path: 'workplace/create', component: WorkplaceCreateComponent}
        ]
    },
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [UserGuard],
        children: [
            {path: 'users', component: UsersComponent}
        ]
    },
    {path: '**', redirectTo: '', pathMatch: 'full'}
]
