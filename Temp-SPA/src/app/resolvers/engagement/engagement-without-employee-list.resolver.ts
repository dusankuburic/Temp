import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Employee } from '../../models/employee';
import { AlertifyService } from '../../services/alertify.service';
import { EngagementService } from '../../services/engagement.service';
import { PaginatedResult } from 'src/app/models/pagination';

@Injectable()
export class EngagementWithoutEmployeeResolver  {

    pageNumber = 1
    pageSize = 5

    constructor(
        private engagementService: EngagementService,
        private router: Router,
        private alertify: AlertifyService){}

    resolve(route: ActivatedRouteSnapshot): Observable<PaginatedResult<Employee[]>> {
        return this.engagementService.getEmployeesWithoutEngagement(this.pageNumber, this.pageSize).pipe(
            catchError(error => {
                this.alertify.error('Problem retrieving data');
                this.router.navigate(['']);
                return of(null);
            })
        );
    }
}