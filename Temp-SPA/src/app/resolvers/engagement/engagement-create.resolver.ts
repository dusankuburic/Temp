import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AlertifyService } from '../../services/alertify.service';
import { EngagementService } from '../../services/engagement.service';

@Injectable()
export class EngagementCreateResolver  {

    constructor(
        private engagementService: EngagementService,
        private router: Router,
        private alertify: AlertifyService){}

    //TODO: pass id into modal, then call db, discard this resolver you don;nt need it
    resolve(route: ActivatedRouteSnapshot): Observable<any> {
        return  this.engagementService.getEngagementForEmployee(route.params['id']).pipe(
            catchError(error => {
                this.alertify.error('Problem retrieving data');
                this.router.navigate(['']);
                return of(null);
            })
        );
    }
}