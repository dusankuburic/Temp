import { Routes } from '@angular/router';
import { AuthGuard } from './_guards/auth.guard';
import { UserGuard } from './_guards/user.guard';
import { EmployeeAssignRoleComponent } from './employee/employee-assign-role/employee-assign-role.component';
import { EmployeeCreateComponent } from './employee/employee-create/employee-create.component';
import { EmployeeEditComponent } from './employee/employee-edit/employee-edit.component';
import { EmployeeListComponent } from './employee/employee-list/employee-list.component';
import { EmploymentStatusCreateComponent } from './employment-status/employment-status-create/employment-status-create.component';
import { EmploymentStatusEditComponent } from './employment-status/employment-status-edit/employment-status-edit.component';
import { EmploymentStatusListComponent } from './employment-status/employment-status-list/employment-status-list.component';
import { EngagementCreateComponent } from './engagement/engagement-create/engagement-create.component';
import { EngagementWithEmployeeListComponent } from './engagement/engagement-with-employee-list/engagement-with-employee-list.component';
import { EngagementWithoutEmployeeListComponent } from './engagement/engagement-without-employee-list/engagement-without-employee-list.component';
import { HomeComponent } from './home/home.component';
import { GroupCreateComponent } from './organization/group/group-create/group-create.component';
import { GroupEditComponent } from './organization/group/group-edit/group-edit.component';
import { GroupListComponent } from './organization/group/inner-group-list/inner-group-list.component';
import { TeamCreateComponent } from './organization/group/team/team-create/team-create.component';
import { TeamEditComponent } from './organization/group/team/team-edit/team-edit.component';
import { TeamListComponent } from './organization/group/team/inner-team-list/inner-team-list.component';
import { OrganizationCreateComponent } from './organization/organization-create/organization-create.component';
import { OrganizationEditComponent } from './organization/organization-edit/organization-edit.component';
import { OrganizationListComponent } from './organization/organization-list/organization-list.component';
import { UsersComponent } from './users/users.component';
import { WorkplaceCreateComponent } from './workplace/workplace-create/workplace-create.component';
import { WorkplaceEditComponent } from './workplace/workplace-edit/workplace-edit.component';
import { WorkplaceListComponent } from './workplace/workplace-list/workplace-list.component';
import { EmployeeEditResolver } from './_resolvers/employee/employee-edit.resolver';
import { EmployeeListResolver } from './_resolvers/employee/employee-list.resolver';
import { EmploymentStatusEditResolver } from './_resolvers/employment-status/employment-status-edit.resolver';
import { EmploymentStatusListResolver } from './_resolvers/employment-status/employment-status-list.resolver';
import { EngagmentCreateResolver } from './_resolvers/engagement/engagement-create.resolver';
import { EngagmentWithEmployeeResolver } from './_resolvers/engagement/engagement-with-employee-list.resolver';
import { EngagmentWithoutEmployeeResolver } from './_resolvers/engagement/engagement-without-employee-list.resolver';
import { GroupCreateResolver } from './_resolvers/group/group-create.resolver';
import { GroupEditResolver } from './_resolvers/group/group-edit.resolver';
import { GroupListResolver } from './_resolvers/group/group-list.resolver';
import { OrganizationEditResolver } from './_resolvers/organization/organization-edit.resolver';
import { OrganizationListResolver } from './_resolvers/organization/organization-list.resolver';
import { TeamCreateResolver } from './_resolvers/team/team-create.resolver';
import { TeamEditResolver } from './_resolvers/team/team-edit.resolver';
import { TeamListResolver } from './_resolvers/team/team-list.resolver';
import { WorkplaceEditResolver } from './_resolvers/workplace/workplace-edit.resolver';
import { WorkplaceListResolver } from './_resolvers/workplace/workplace-list.resolver';
import { ModeratorGuard } from './_guards/moderator.guard';
import { ModeratorComponent } from './moderator/moderator.component';
import { AssignedGroupsComponent } from './assigned-groups/assigned-groups.component';
import { ModeratorAssignedGroupsResolver } from './_resolvers/group/moderator-assigned-groups.resolver';
import { AssignedInnerTeamsComponent } from './assigned-groups/assigned-inner-teams/assigned-inner-teams.component';
import { ApplicationCreateComponent } from './application/application-create/application-create.component';
import { ApplicationCreateResolver } from './_resolvers/application/application-create.resolver';
import { ApplicationModeratorListComponent } from './application/application-moderator-list/application-moderator-list.component';
import { ApplicationModeratorListResolver } from './_resolvers/application/application-moderator-list.resolver';
import { ApplicationUserListComponent } from './application/application-user-list/application-user-list.component';
import { ApplicationUserListResolver } from './_resolvers/application/application-user-list.resolver';
import { ApplicationUserComponent } from './application/application-user/application-user.component';
import { ApplicationUserResolver } from './_resolvers/application/application-user.resolver';
import { EngagementUserListComponent } from './engagement/engagement-user-list/engagement-user-list.component';
import { EngagementUserListReslover } from './_resolvers/engagement/engagement-user-list.resolver';
import { ApplicationModeratorResolver } from './_resolvers/application/application-moderator.resolver';
import { ApplicationModeratorComponent } from './application/application-moderator/application-moderator.component';

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
                resolve: {
                    employee: EmployeeEditResolver,
                    organizations: OrganizationListResolver
                }},
            {path: 'employee/create', component: EmployeeCreateComponent,
                resolve: {organizations: OrganizationListResolver}},
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

            {path: 'organization/inner-groups/:id', component: GroupListComponent,
                resolve: {innergroups: GroupListResolver}},
            {path: 'group/create/:id', component: GroupCreateComponent,
                resolve: {organization: GroupCreateResolver}},
            {path: 'groups/:id', component: GroupEditComponent,
                resolve: {group: GroupEditResolver}},

            {path: 'groups/inner-teams/:id', component: TeamListComponent,
                resolve: {innerteams: TeamListResolver}},
            {path: 'team/create/:id', component: TeamCreateComponent,
                resolve: {group: TeamCreateResolver}},
            {path: 'teams/:id', component: TeamEditComponent,
                resolve: {team: TeamEditResolver}}
        ]
    },
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [UserGuard],
        children: [
            {path: 'users', component: UsersComponent},
            {path: 'application/create/:id', component: ApplicationCreateComponent,
                resolve: {team: ApplicationCreateResolver}},
            {path: 'application-list/:id', component: ApplicationUserListComponent,
                resolve: {applications: ApplicationUserListResolver}},
            {path: 'application-user/:id', component: ApplicationUserComponent,
                resolve: {application: ApplicationUserResolver}},
            {path: 'engagement-user-list/:id', component: EngagementUserListComponent,
                resolve: {engagements: EngagementUserListReslover}}
        ]
    },
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [ModeratorGuard],
        children: [
            {path: 'moderators', component: ModeratorComponent},
            {path: 'assigned-groups/:id', component: AssignedGroupsComponent,
                resolve: {groups: ModeratorAssignedGroupsResolver}},
            {path: 'assigned-groups/inner-teams/:id', component: AssignedInnerTeamsComponent,
                resolve: {teams: TeamListResolver}},
            {path: 'application-list/team/:id', component: ApplicationModeratorListComponent,
                resolve: {applications: ApplicationModeratorListResolver}},
            {path: 'application-moderator/:id', component: ApplicationModeratorComponent,
                resolve: {application: ApplicationModeratorResolver}},
        ]
    },
    {path: '**', redirectTo: '', pathMatch: 'full'}
];
