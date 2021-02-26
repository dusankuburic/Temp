import { Routes } from '@angular/router';
import { EmployeeAssignRoleComponent } from './employee/employee-assign-role/employee-assign-role.component';
import { EmployeeCreateComponent } from './employee/employee-create/employee-create.component';
import { EmployeeEditComponent } from './employee/employee-edit/employee-edit.component';
import { EmployeeListComponent } from './employee/employee-list/employee-list.component';
import { EmploymentStatusCreateComponent } from './employment-status/employment-status-create/employment-status-create.component';
import { EmploymentStatusEditComponent } from './employment-status/employment-status-edit/employment-status-edit.component';
import { EmploymentStatusListComponent } from './employment-status/employment-status-list/employment-status-list.component';
import { EngagementCreateComponent } from './engagemet/engagement-create/engagement-create.component';
import { EngagementWithEmployeeListComponent } from './engagemet/engagement-with-employee-list/engagement-with-employee-list.component';
import { EngagementWithoutEmployeeListComponent } from './engagemet/engagement-without-employee-list/engagement-without-employee-list.component';
import { HomeComponent } from './home/home.component';
import { GroupListComponent } from './organization/group/group-list/group-list.component';
import { OrganizationCreateComponent } from './organization/organization-create/organization-create.component';
import { OrganizationEditComponent } from './organization/organization-edit/organization-edit.component';
import { OrganizationListComponent } from './organization/organization-list/organization-list.component';
import { UsersComponent } from './users/users.component';
import { WorkplaceCreateComponent } from './workplace/workplace-create/workplace-create.component';
import { WorkplaceEditComponent } from './workplace/workplace-edit/workplace-edit.component';
import { WorkplaceListComponent } from './workplace/workplace-list/workplace-list.component';
import { AuthGuard } from './_guards/auth.guard';
import { UserGuard } from './_guards/user.guard';
import { EmployeeEditResolver } from './_resolvers/employee-edit.resolver';
import { EmployeeListResolver } from './_resolvers/employee-list.resolver';
import { EmploymentStatusEditResolver } from './_resolvers/employment-status-edit.resolver';
import { EmploymentStatusListResolver } from './_resolvers/employment-status-list.resolver';
import { EngagmentCreateResolver } from './_resolvers/engagement-create.resolver';
import { EngagmentWithEmployeeResolver } from './_resolvers/engagement-with-employee-list.resolver';
import { EngagmentWithoutEmployeeResolver } from './_resolvers/engagement-without-employee-list.resolver';
import { GroupListResolver } from './_resolvers/group-list.resolver';
import { OrganizationEditResolver } from './_resolvers/organization-edit.resolver';
import { OrganizationListResolver } from './_resolvers/organization-list.resolver';
import { WorkplaceEditResolver } from './_resolvers/workplace-edit.resolver';
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
            {path: 'workplaces/:id', component: WorkplaceEditComponent,
                resolve: {workplace: WorkplaceEditResolver}},
            {path: 'workplace/create', component: WorkplaceCreateComponent},

            {path: 'employment-statuses', component: EmploymentStatusListComponent,
                resolve: {employmentStatuses: EmploymentStatusListResolver}},
            {path: 'employment-statuses/:id', component: EmploymentStatusEditComponent,
                resolve: {employmentStatus: EmploymentStatusEditResolver}},
            {path: 'employment-status/create', component: EmploymentStatusCreateComponent},

            {path: 'engagement/create/:id', component: EngagementCreateComponent,
                resolve: {employeeData: EngagmentCreateResolver}},
            {path: 'engagement/with-employee', component: EngagementWithEmployeeListComponent,
                resolve: {employeesWith: EngagmentWithEmployeeResolver}},
            {path: 'engagement/without-employee', component: EngagementWithoutEmployeeListComponent,
                resolve: {employeesWithout: EngagmentWithoutEmployeeResolver}},

            {path: 'organizations', component: OrganizationListComponent,
                resolve: {organizations: OrganizationListResolver}},
            {path: 'organizations/:id', component: OrganizationEditComponent,
                resolve: {organization: OrganizationEditResolver}},
            {path: 'organization/create', component: OrganizationCreateComponent},

            {path: 'inner-groups/:id', component: GroupListComponent,
                resolve: {groups: GroupListResolver}}
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
