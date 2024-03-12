import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Employee } from '../../models/employee';
import { AlertifyService } from '../../services/alertify.service';
import { EmployeeService } from '../../services/employee.service';
import { PaginatedResult } from 'src/app/core/models/pagination';

@Injectable()
export class EmployeeListResolver {
    constructor(
        private employeeService: EmployeeService,
        private router: Router,
        private alertify: AlertifyService){}

    resolve(): Observable<PaginatedResult<Employee[]>> {
        this.employeeService.resetEmployeeParams();
        return this.employeeService.getEmployees().pipe(
            catchError(() => {
                this.alertify.error('Unable to list Employees');
                this.router.navigate(['']);
                return of(null);
            })
        );
    }
}