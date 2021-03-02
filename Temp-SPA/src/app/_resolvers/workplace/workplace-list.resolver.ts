import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Workplace } from '../../_models/workplace';
import { AlertifyService } from '../../_services/alertify.service';
import { WorkplaceService } from '../../_services/workplace.service';

@Injectable()
export class WorkplaceListResolver implements Resolve<Workplace[]> {

    pageNumber = 1;
    pageSize = 5;

    constructor(
        private workplaceService: WorkplaceService,
        private router: Router,
        private alertify: AlertifyService){}

    resolve(route: ActivatedRouteSnapshot): Observable<Workplace[]> {
        return this.workplaceService.getWorkplaces(this.pageNumber, this.pageSize).pipe(
            catchError(error => {
                this.alertify.error('Problem retriving data');
                this.router.navigate(['']);
                return of(null);
            })
        );
    }
}