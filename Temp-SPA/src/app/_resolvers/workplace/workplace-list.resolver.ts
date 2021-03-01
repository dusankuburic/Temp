import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Workplace } from '../../_models/workplace';
import { AlertifyService } from '../../_services/alertify.service';
import { WorkplaceService } from '../../_services/workplace.service';

@Injectable()
export class WorkplaceListResolver implements Resolve<Workplace[]> {

    constructor(
        private workplaceService: WorkplaceService,
        private router: Router,
        private alertify: AlertifyService){}

    resolve(route: ActivatedRouteSnapshot): Observable<Workplace[]> {
        return this.workplaceService.getWorkplaces().pipe(
            catchError(error => {
                this.alertify.error('Problem retriving data');
                this.router.navigate(['']);
                return of(null);
            })
        );
    }
}