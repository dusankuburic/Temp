import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Employee } from '../../models/employee';
import { AlertifyService } from '../../services/alertify.service';
import { EmployeeService } from '../../services/employee.service';

@Injectable()
export class EmployeeEditResolver implements Resolve<Employee> {

    constructor(
        private employeeService: EmployeeService,
        private router: Router,
        private alertify: AlertifyService) {}


    resolve(route: ActivatedRouteSnapshot): Observable<Employee>{
        return this.employeeService.getEmployee(route.params['id']).pipe(
            catchError(() => {
                this.alertify.error('Unable to get Employee');
                this.router.navigate(['/employees']);
                return of(null);
            })
        );
    }
}