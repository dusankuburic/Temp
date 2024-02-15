import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Employee } from '../../models/employee';
import { AlertifyService } from '../../services/alertify.service';
import { EmployeeService } from '../../services/employee.service';

@Injectable()
export class EmployeeEditResolver  {

    constructor(
        private employeeService: EmployeeService,
        private router: Router,
        private alertify: AlertifyService) {}


    resolve(route: ActivatedRouteSnapshot): Observable<Employee>{
        return this.employeeService.getEmployee(route.params['id']).pipe(
            catchError(error => {
                this.alertify.error('Problem retrieving data');
                this.router.navigate(['/employees']);
                return of(null);
            })
        );
    }
}