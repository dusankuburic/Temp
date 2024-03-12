import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { UserEngagement } from 'src/app/core/models/engagement';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EngagementService } from 'src/app/core/services/engagement.service';

@Injectable()
export class EngagementUserListResolver  {

    constructor(
        private engagementService: EngagementService,
        private route: Router,
        private alertify: AlertifyService){}

    resolve(route: ActivatedRouteSnapshot): Observable<UserEngagement[]> {
        return this.engagementService.getUserEmployeeEngagements(route.params['id']).pipe(
            catchError(() => {
                this.alertify.error('Unable to list Employee Engagements');
                this.route.navigate(['']);
                return of(null);
            })
        );
    }
}