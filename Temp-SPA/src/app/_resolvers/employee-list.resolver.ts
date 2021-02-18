import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Employee } from '../_models/employee';
import { AlertifyService } from '../_services/alertify.service';
import { EmployeeService } from '../_services/employee.service';

@Injectable()
export class EmployeeListResolver implements Resolve<Employee[]> {

    constructor(
        private employeeService: EmployeeService,
        private router: Router,
        private alertify: AlertifyService){}

    resolve(route: ActivatedRouteSnapshot): Observable<Employee[]> {
        return this.employeeService.getEmployees().pipe(
            catchError(error => {
                this.alertify.error('Problem retriving data');
                this.router.navigate(['']);
                return of(null);
            })
        );
    }
}