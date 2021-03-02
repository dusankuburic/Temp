import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Employee } from '../../_models/employee';
import { AlertifyService } from '../../_services/alertify.service';
import { EngagementService } from '../../_services/engagement.service';

@Injectable()
export class EngagmentWithoutEmployeeResolver implements Resolve<Employee[]> {

    pageNumber = 1
    pageSize = 5

    constructor(
        private engaementService: EngagementService,
        private router: Router,
        private alertify: AlertifyService){}

    resolve(route: ActivatedRouteSnapshot): Observable<Employee[]> {
        return this.engaementService.getEmpoyeesWithoutEngagement(this.pageNumber, this.pageSize).pipe(
            catchError(error => {
                this.alertify.error('Problem retriving data');
                this.router.navigate(['']);
                return of(null);
            })
        );
    }
}