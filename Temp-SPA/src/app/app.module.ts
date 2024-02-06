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
import { AuthService } from './services/auth.service';
import { AlertifyService } from './services/alertify.service';
import { HomeComponent } from './home/home.component';
import { appRoutes } from './routes';
import { LoginComponent } from './login/login.component';


import { UsersComponent } from './users/users.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { EmployeeListResolver } from './resolvers/employee/employee-list.resolver';
import { EmployeeEditResolver } from './resolvers/employee/employee-edit.resolver';
import { WorkplaceListResolver } from './resolvers/workplace/workplace-list.resolver';
import { WorkplaceEditResolver } from './resolvers/workplace/workplace-edit.resolver';
import { EmploymentStatusListResolver } from './resolvers/employment-status/employment-status-list.resolver';
import { EmploymentStatusEditResolver } from './resolvers/employment-status/employment-status-edit.resolver';
import { OrganizationListResolver } from './resolvers/organization/organization-list.resolver';
import { OrganizationEditResolver } from './resolvers/organization/organization-edit.resolver';
import { GroupListResolver } from './resolvers/group/group-list.resolver';
import { GroupCreateResolver } from './resolvers/group/group-create.resolver';
import { GroupEditResolver } from './resolvers/group/group-edit.resolver';
import { TeamListResolver } from './resolvers/team/team-list.resolver';
import { TeamCreateResolver } from './resolvers/team/team-create.resolver';
import { TeamEditResolver } from './resolvers/team/team-edit.resolver';
import { ModeratorComponent } from './moderator/moderator.component';
import { ModeratorAssignedGroupsResolver } from './resolvers/group/moderator-assigned-groups.resolver';
import { ApplicationCreateResolver } from './resolvers/application/application-create.resolver';
import { ApplicationModeratorListResolver } from './resolvers/application/application-moderator-list.resolver';
import { ApplicationUserListResolver } from './resolvers/application/application-user-list.resolver';
import { ApplicationUserResolver } from './resolvers/application/application-user.resolver';
import { EngagementUserListResolver } from './resolvers/engagement/engagement-user-list.resolver';
import { ApplicationModeratorResolver } from './resolvers/application/application-moderator.resolver';
import { ApplicationModule } from './application/application.module';
import { EmployeeModule } from './employee/employee.module';
import { EmploymentStatusModule } from './employment-status/employment-status.module';
import { EngagementModule } from './engagement/engagement.module';
import { WorkplaceModule } from './workplace/workplace.module';
import { GroupModule } from './group/group.module';
import { TeamModule } from './team/team.module';
import { OrganizationModule } from './organization/organization.module';
import { EngagementWithEmployeeResolver } from './resolvers/engagement/engagement-with-employee-list.resolver';
import { EngagementWithoutEmployeeResolver } from './resolvers/engagement/engagement-without-employee-list.resolver';
import { EngagementCreateResolver } from './resolvers/engagement/engagement-create.resolver';

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
    ModeratorComponent,
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
    }),
    ApplicationModule,
    EmployeeModule,
    EmploymentStatusModule,
    EngagementModule,
    WorkplaceModule,
    GroupModule,
    TeamModule,
    OrganizationModule,
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
    EngagementWithEmployeeResolver,
    EngagementWithoutEmployeeResolver,
    EngagementUserListResolver,
    EngagementCreateResolver,
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
    ApplicationUserResolver,
    ApplicationModeratorResolver,
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
