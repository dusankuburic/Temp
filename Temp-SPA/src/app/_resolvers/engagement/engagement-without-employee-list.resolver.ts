import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Employee } from '../../_models/employee';
import { AlertifyService } from '../../_services/alertify.service';
import { EngagementService } from '../../_services/engagement.service';
import { PaginatedResult } from 'src/app/_models/pagination';

@Injectable()
export class EngagmentWithoutEmployeeResolver  {

    pageNumber = 1
    pageSize = 5

    constructor(
        private engaementService: EngagementService,
        private router: Router,
        private alertify: AlertifyService){}

    resolve(route: ActivatedRouteSnapshot): Observable<PaginatedResult<Employee[]>> {
        return this.engaementService.getEmpoyeesWithoutEngagement(this.pageNumber, this.pageSize).pipe(
            catchError(error => {
                this.alertify.error('Problem retriving data');
                this.router.navigate(['']);
                return of(null);
            })
        );
    }
}