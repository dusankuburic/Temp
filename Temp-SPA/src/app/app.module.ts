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
import { AuthService } from './core/services/auth.service';
import { AlertifyService } from './core/services/alertify.service';
import { HomeComponent } from './home/home.component';
import { appRoutes } from './routes';
import { LoginComponent } from './login/login.component';


import { UsersComponent } from './users/users.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { EmployeeListResolver } from './core/resolvers/employee/employee-list.resolver';
import { EmployeeEditResolver } from './core/resolvers/employee/employee-edit.resolver';
import { WorkplaceListResolver } from './core/resolvers/workplace/workplace-list.resolver';
import { WorkplaceEditResolver } from './core/resolvers/workplace/workplace-edit.resolver';
import { EmploymentStatusListResolver } from './core/resolvers/employment-status/employment-status-list.resolver';
import { EmploymentStatusEditResolver } from './core/resolvers/employment-status/employment-status-edit.resolver';
import { OrganizationListResolver } from './core/resolvers/organization/organization-list.resolver';
import { OrganizationEditResolver } from './core/resolvers/organization/organization-edit.resolver';
import { GroupListResolver } from './core/resolvers/group/group-list.resolver';
import { GroupCreateResolver } from './core/resolvers/group/group-create.resolver';
import { GroupEditResolver } from './core/resolvers/group/group-edit.resolver';
import { TeamListResolver } from './core/resolvers/team/team-list.resolver';
import { TeamCreateResolver } from './core/resolvers/team/team-create.resolver';
import { TeamEditResolver } from './core/resolvers/team/team-edit.resolver';
import { ModeratorComponent } from './moderator/moderator.component';
import { ModeratorAssignedGroupsResolver } from './core/resolvers/group/moderator-assigned-groups.resolver';
import { ApplicationCreateResolver } from './core/resolvers/application/application-create.resolver';
import { ApplicationModeratorListResolver } from './core/resolvers/application/application-moderator-list.resolver';
import { ApplicationUserListResolver } from './core/resolvers/application/application-user-list.resolver';
import { ApplicationUserResolver } from './core/resolvers/application/application-user.resolver';
import { EngagementUserListResolver } from './core/resolvers/engagement/engagement-user-list.resolver';
import { ApplicationModeratorResolver } from './core/resolvers/application/application-moderator.resolver';
import { ApplicationModule } from './application/application.module';
import { EmployeeModule } from './employee/employee.module';
import { EmploymentStatusModule } from './employment-status/employment-status.module';
import { EngagementModule } from './engagement/engagement.module';
import { WorkplaceModule } from './workplace/workplace.module';
import { GroupModule } from './group/group.module';
import { TeamModule } from './team/team.module';
import { OrganizationModule } from './organization/organization.module';
import { EngagementWithEmployeeResolver } from './core/resolvers/engagement/engagement-with-employee-list.resolver';
import { EngagementWithoutEmployeeResolver } from './core/resolvers/engagement/engagement-without-employee-list.resolver';
import { EngagementCreateResolver } from './core/resolvers/engagement/engagement-create.resolver';

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
