import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Workplace } from '../../models/workplace';
import { AlertifyService } from '../../services/alertify.service';
import { WorkplaceService } from '../../services/workplace.service';
import { PaginatedResult } from 'src/app/core/models/pagination';

@Injectable()
export class WorkplaceListResolver  {
    constructor(
        private workplaceService: WorkplaceService,
        private router: Router,
        private alertify: AlertifyService){}

    resolve(route: ActivatedRouteSnapshot): Observable<PaginatedResult<Workplace[]>>  {
        this.workplaceService.resetWorkplaceParams();
        return this.workplaceService.getPagedWorkplaces().pipe(
            catchError(error => {
                this.alertify.error('Unable to list Workplaces');
                this.router.navigate(['']);
                return of(null);
            })
        );
    }
}