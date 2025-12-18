import { Injectable } from '@angular/core';
import { Resolve, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { EmploymentStatus } from '../../models/employmentStatus';
import { AlertifyService } from '../../services/alertify.service';
import { EmploymentStatusService } from '../../services/employment-status.service';
import { PaginatedResult } from 'src/app/core/models/pagination';

@Injectable()
export class EmploymentStatusListResolver implements Resolve<PaginatedResult<EmploymentStatus[]> | null> {
    constructor(
        private employmentStatusService: EmploymentStatusService,
        private router: Router,
        private alertify: AlertifyService) {}

    resolve(): Observable<PaginatedResult<EmploymentStatus[]> | null> {
        this.employmentStatusService.resetEmploymentStatusParams();
        return this.employmentStatusService.getPagedEmploymentStatuses().pipe(
            catchError(() => {
                this.alertify.error('Unable to list Employment statuses');
                this.router.navigate(['']);
                return of(null);
            })
        );
    }
}