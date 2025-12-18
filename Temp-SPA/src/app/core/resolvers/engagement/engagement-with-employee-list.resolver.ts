import { Injectable } from '@angular/core';
import { Resolve, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Employee } from '../../models/employee';
import { AlertifyService } from '../../services/alertify.service';
import { EngagementService } from '../../services/engagement.service';
import { PaginatedResult } from 'src/app/core/models/pagination';

@Injectable()
export class EngagementWithEmployeeResolver implements Resolve<PaginatedResult<Employee[]> | null> {
    constructor(
        private engagementService: EngagementService,
        private router: Router,
        private alertify: AlertifyService){}

    resolve(): Observable<PaginatedResult<Employee[]> | null> {
        this.engagementService.resetEngagementParams();
        return this.engagementService.getEmployeesWithEngagement().pipe(
            catchError(() => {
                this.alertify.error('Unable to list Employees');
                this.router.navigate(['']);
                return of(null);
            })
        );
    }
}