import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { JwtModule } from '@auth0/angular-jwt';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import {BsDatepickerModule} from 'ngx-bootstrap/datepicker';
import { RouterModule } from '@angular/router';
import {PaginationModule} from 'ngx-bootstrap/pagination';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { AuthService } from './_services/auth.service';
import { AlertifyService } from './_services/alertify.service';
import { HomeComponent } from './home/home.component';
import { appRoutes } from './routes';
import { LoginComponent } from './login/login.component';


import { UsersComponent } from './users/users.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { EmployeeListResolver } from './_resolvers/employee/employee-list.resolver';
import { EmployeeEditResolver } from './_resolvers/employee/employee-edit.resolver';
import { EmployeeCreateComponent } from './employee/employee-create/employee-create.component';
import { EmployeeEditComponent } from './employee/employee-edit/employee-edit.component';
import { EmployeeAssignRoleComponent } from './employee/employee-assign-role/employee-assign-role.component';
import { WorkplaceListComponent } from './workplace/workplace-list/workplace-list.component';
import { EmployeeListComponent } from './employee/employee-list/employee-list.component';
import { WorkplaceListResolver } from './_resolvers/workplace/workplace-list.resolver';
import { WorkplaceCreateComponent } from './workplace/workplace-create/workplace-create.component';
import { WorkplaceEditComponent } from './workplace/workplace-edit/workplace-edit.component';
import { WorkplaceEditResolver } from './_resolvers/workplace/workplace-edit.resolver';
import { EmploymentStatusListComponent } from './employment-status/employment-status-list/employment-status-list.component';
import { EmploymentStatusListResolver } from './_resolvers/employment-status/employment-status-list.resolver';
import { EmploymentStatusCreateComponent } from './employment-status/employment-status-create/employment-status-create.component';
import { EmploymentStatusEditComponent } from './employment-status/employment-status-edit/employment-status-edit.component';
import { EmploymentStatusEditResolver } from './_resolvers/employment-status/employment-status-edit.resolver';
import { EngagementWithEmployeeListComponent } from './engagement/engagement-with-employee-list/engagement-with-employee-list.component';
import { EngagementWithoutEmployeeListComponent } from './engagement/engagement-without-employee-list/engagement-without-employee-list.component';
import { EngagmentWithEmployeeResolver } from './_resolvers/engagement/engagement-with-employee-list.resolver';
import { EngagmentWithoutEmployeeResolver } from './_resolvers/engagement/engagement-without-employee-list.resolver';
import { EngagementCreateComponent } from './engagement/engagement-create/engagement-create.component';
import { EngagmentCreateResolver } from './_resolvers/engagement/engagement-create.resolver';
import { OrganizationListComponent } from './organization/organization-list/organization-list.component';
import { OrganizationListResolver } from './_resolvers/organization/organization-list.resolver';
import { OrganizationEditComponent } from './organization/organization-edit/organization-edit.component';
import { OrganizationEditResolver } from './_resolvers/organization/organization-edit.resolver';
import { OrganizationCreateComponent } from './organization/organization-create/organization-create.component';
import { GroupListComponent } from './organization/group/inner-group-list/inner-group-list.component';
import { GroupListResolver } from './_resolvers/group/group-list.resolver';
import { GroupCreateComponent } from './organization/group/group-create/group-create.component';
import { GroupCreateResolver } from './_resolvers/group/group-create.resolver';
import { GroupEditComponent } from './organization/group/group-edit/group-edit.component';
import { GroupEditResolver } from './_resolvers/group/group-edit.resolver';
import { TeamListComponent } from './organization/group/team/inner-team-list/inner-team-list.component';
import { TeamListResolver } from './_resolvers/team/team-list.resolver';
import { TeamCreateComponent } from './organization/group/team/team-create/team-create.component';
import { TeamCreateResolver } from './_resolvers/team/team-create.resolver';
import { TeamEditComponent } from './organization/group/team/team-edit/team-edit.component';
import { TeamEditResolver } from './_resolvers/team/team-edit.resolver';
import { ModeratorComponent } from './moderator/moderator.component';
import { AssignedGroupsComponent } from './assigned-groups/assigned-groups.component';
import { ModeratorAssignedGroupsResolver } from './_resolvers/group/moderator-assigned-groups.resolver';
import { AssignedInnerTeamsComponent } from './assigned-groups/assigned-inner-teams/assigned-inner-teams.component';
import { ApplicationCreateComponent } from './application/application-create/application-create.component';
import { ApplicationCreateResolver } from './_resolvers/application/application-create.resolver';
import { ApplicationModeratorListResolver } from './_resolvers/application/application-moderator-list.resolver';
import { ApplicationModeratorListComponent } from './application/application-moderator-list/application-moderator-list.component';
import { ApplicationUserListComponent } from './application/application-user-list/application-user-list.component';
import { ApplicationUserListResolver } from './_resolvers/application/application-user-list.resolver';
import { ApplicationUserResolver } from './_resolvers/application/application-user.resolver';
import { ApplicationUserComponent } from './application/application-user/application-user.component';


export function tokenGetter(): any {
  return localStorage.getItem('token');
}

@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    HomeComponent,
    LoginComponent,
    SidebarComponent,
    UsersComponent,
    EmployeeListComponent,
    EmployeeCreateComponent,
    EmployeeEditComponent,
    EmployeeAssignRoleComponent,
    WorkplaceListComponent,
    WorkplaceCreateComponent,
    WorkplaceEditComponent,
    EmploymentStatusListComponent,
    EmploymentStatusCreateComponent,
    EmploymentStatusEditComponent,
    EngagementWithEmployeeListComponent,
    EngagementWithoutEmployeeListComponent,
    EngagementCreateComponent,
    OrganizationListComponent,
    OrganizationEditComponent,
    OrganizationCreateComponent,
    GroupListComponent,
    GroupCreateComponent,
    GroupEditComponent,
    TeamListComponent,
    TeamCreateComponent,
    TeamEditComponent,
    ModeratorComponent,
    AssignedGroupsComponent,
    AssignedInnerTeamsComponent,
    ApplicationCreateComponent,
    ApplicationModeratorListComponent,
    ApplicationUserListComponent,
    ApplicationUserComponent
   ],
  imports: [
    RouterModule.forRoot(appRoutes),
    BrowserModule,
    FormsModule,
    AppRoutingModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    PaginationModule.forRoot(),
    BsDropdownModule.forRoot(),
    BsDatepickerModule.forRoot(),
    HttpClientModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        allowedDomains: ['localhost:5000'],
        disallowedRoutes: ['localhost:5000/api/admins/register', 'localhost:5000/api/users/register']
      }
    })
  ],
  providers: [
    AuthService,
    AlertifyService,
    EmployeeListResolver,
    EmployeeEditResolver,
    WorkplaceListResolver,
    WorkplaceEditResolver,
    EmploymentStatusListResolver,
    EmploymentStatusEditResolver,
    EngagmentWithEmployeeResolver,
    EngagmentWithoutEmployeeResolver,
    EngagmentCreateResolver,
    OrganizationListResolver,
    OrganizationEditResolver,
    GroupListResolver,
    GroupCreateResolver,
    GroupEditResolver,
    TeamListResolver,
    TeamCreateResolver,
    TeamEditResolver,
    ModeratorAssignedGroupsResolver,
    ApplicationCreateResolver,
    ApplicationModeratorListResolver,
    ApplicationUserListResolver,
    ApplicationUserResolver
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
