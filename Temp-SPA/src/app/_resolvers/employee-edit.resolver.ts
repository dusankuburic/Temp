import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Employee } from '../_models/employee';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { EmployeeService } from '../_services/employee.service';

@Injectable()
export class EmployeeEditResolver implements Resolve<Employee> {

    constructor(
        private employeeService: EmployeeService,
        private router: Router,
        private alertify: AlertifyService) {}


    resolve(route: ActivatedRouteSnapshot): Observable<Employee>{
        return this.employeeService.getEmployee(route.params['id']).pipe(
            catchError(error => {
                this.alertify.error('Problem retriving data');
                this.router.navigate(['/employees']);
                return of(null);
            })
        );
    }
}