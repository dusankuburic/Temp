import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Workplace } from '../../models/workplace';
import { AlertifyService } from '../../services/alertify.service';
import { WorkplaceService } from '../../services/workplace.service';

@Injectable()
export class WorkplaceEditResolver  {

    constructor(
        private workplaceService: WorkplaceService,
        private router: Router,
        private alertify: AlertifyService){}

    resolve(route: ActivatedRouteSnapshot): Observable<Workplace> {
        return this.workplaceService.getWorkplace(route.params['id']).pipe(
            catchError(() => {
                this.alertify.error('Unable to get Workplace');
                this.router.navigate(['/employees']);
                return of(null);
            })
        );
    }
}