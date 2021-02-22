import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { JwtModule } from '@auth0/angular-jwt';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { RouterModule } from '@angular/router';

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
import { EmployeeListResolver } from './_resolvers/employee-list.resolver';
import { EmployeeEditResolver } from './_resolvers/employee-edit.resolver';
import { EmployeeCreateComponent } from './employee/employee-create/employee-create.component';
import { EmployeeEditComponent } from './employee/employee-edit/employee-edit.component';
import { EmployeeAssignRoleComponent } from './employee/employee-assign-role/employee-assign-role.component';
import { WorkplaceListComponent } from './workplace/workplace-list/workplace-list.component';
import { EmployeeListComponent } from './employee/employee-list/employee-list.component';
import { WorkplaceListResolver } from './_resolvers/workplace-list.resolver';
import { WorkplaceCreateComponent } from './workplace/workplace-create/workplace-create.component';
import { WorkplaceEditComponent } from './workplace/workplace-edit/workplace-edit.component';
import { WorkplaceEditResolver } from './_resolvers/workplace-edit.resolver';
import { EmploymentStatusListComponent } from './employment-status/employment-status-list/employment-status-list.component';
import { EmploymentStatusListResolver } from './_resolvers/employment-status-list.resolver';
import { EmploymentStatusCreateComponent } from './employment-status/employment-status-create/employment-status-create.component';
import { EmploymentStatusEditComponent } from './employment-status/employment-status-edit/employment-status-edit.component';
import { EmploymentStatusEditResolver } from './_resolvers/employment-status-edit.resolver';
import { EngagementWithEmployeeListComponent } from './engagemet/engagement-with-employee-list/engagement-with-employee-list.component';
import { EngagementWithoutEmployeeListComponent } from './engagemet/engagement-without-employee-list/engagement-without-employee-list.component';
import { EngagmentWithEmployeeResolver } from './_resolvers/engagement-with-employee-list.resolver';
import { EngagmentWithoutEmployeeResolver } from './_resolvers/engagement-without-employee-list.resolver';
import { EngagementCreateComponent } from './engagemet/engagement-create/engagement-create.component';
import { EngagmentCreateResolver } from './_resolvers/engagement-create.resolver';


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
    EmployeeListComponent,
    UsersComponent,
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
    EngagementCreateComponent
   ],
  imports: [
    RouterModule.forRoot(appRoutes),
    BrowserModule,
    FormsModule,
    AppRoutingModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    BsDropdownModule.forRoot(),
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
    EngagmentCreateResolver
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
