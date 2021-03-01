import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { EmploymentStatus } from '../../_models/employmentStatus';
import { AlertifyService } from '../../_services/alertify.service';
import { EmploymentStatusService } from '../../_services/employment-status.service';

@Injectable()
export class EmploymentStatusEditResolver implements Resolve<EmploymentStatus> {

    constructor(
        private employmentStatusService: EmploymentStatusService,
        private router: Router,
        private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<EmploymentStatus> {
        return this.employmentStatusService.getEmploymentStatus(route.params['id']).pipe(
            catchError(error => {
                this.alertify.error('Problem retriving data');
                this.router.navigate(['/employment-statuses']);
                return of(null);
            })
        );
    }
}