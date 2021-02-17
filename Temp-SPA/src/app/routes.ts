import { Routes } from '@angular/router';
import { EmployeeCreateComponent } from './employees/employee-create/employee-create.component';
import { EmployeeEditComponent } from './employees/employee-edit/employee-edit.component';
import { EmployeeListComponent } from './employees/employee-list/employee-list.component';
import { HomeComponent } from './home/home.component';
import { UsersComponent } from './users/users.component';
import { AuthGuard } from './_guards/auth.guard';
import { UserGuard } from './_guards/user.guard';
import { EmployeeEditResolver } from './_resolvers/employee-edit.resolver';
import { EmployeeListResolver } from './_resolvers/employee-list.resolver';

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
            {path: 'employee/create', component: EmployeeCreateComponent}
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
