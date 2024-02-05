import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { UserEngagement } from 'src/app/_models/engagement';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { EngagementService } from 'src/app/_services/engagement.service';

@Injectable()
export class EngagementUserListReslover  {

    constructor(
        private engagementService: EngagementService,
        private route: Router,
        private alertify: AlertifyService){}

    resolve(route: ActivatedRouteSnapshot): Observable<UserEngagement[]> {
        return this.engagementService.getUserEmployeeEngagments(route.params['id']).pipe(
            catchError(error => {
                this.alertify.error('Problem retriving data');
                this.route.navigate(['']);
                return of(null);
            })
        );
    }
}