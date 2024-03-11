import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Employee } from '../../models/employee';
import { AlertifyService } from '../../services/alertify.service';
import { EngagementService } from '../../services/engagement.service';
import { PaginatedResult } from 'src/app/core/models/pagination';

@Injectable()
export class EngagementWithEmployeeResolver  {
    constructor(
        private engagementService: EngagementService,
        private router: Router,
        private alertify: AlertifyService){}

    resolve(): Observable<PaginatedResult<Employee[]>> {
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