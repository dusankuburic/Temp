import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { EmploymentStatus } from '../../models/employmentStatus';
import { AlertifyService } from '../../services/alertify.service';
import { EmploymentStatusService } from '../../services/employment-status.service';
import { PaginatedResult } from 'src/app/core/models/pagination';

@Injectable()
export class EmploymentStatusListResolver  {
    constructor(
        private employmentStatusService: EmploymentStatusService,
        private router: Router,
        private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<PaginatedResult<EmploymentStatus[]>> {
        this.employmentStatusService.resetEmploymentStatusParams();
        return this.employmentStatusService.getPagedEmploymentStatuses().pipe(
            catchError(error => {
                this.alertify.error('Problem retrieving data');
                this.router.navigate(['']);
                return of(null);
            })
        );
    }
}