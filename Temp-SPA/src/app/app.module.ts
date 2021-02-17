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

import { EmployeeListComponent } from './employees/employee-list/employee-list.component';
import { UsersComponent } from './users/users.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { EmployeeListResolver } from './_resolvers/employee-list.resolver';
import { EmployeeEditResolver } from './_resolvers/employee-edit.resolver';
import { EmployeeCreateComponent } from './employees/employee-create/employee-create.component';
import { EmployeeEditComponent } from './employees/employee-edit/employee-edit.component';


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
    SidebarComponent,
    EmployeeCreateComponent,
    EmployeeEditComponent
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
        disallowedRoutes: ['localhost:5000/api/auth']
      }
    })
  ],
  providers: [
    AuthService,
    AlertifyService,
    EmployeeListResolver,
    EmployeeEditResolver
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
