import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { InnerGroups } from '../../_models/group';
import { AlertifyService } from '../../_services/alertify.service';
import { GroupService } from '../../_services/group.service';

@Injectable()
export class GroupListResolver implements Resolve<InnerGroups> {

    constructor(
        private groupService: GroupService,
        private router: Router,
        private alertify: AlertifyService){}

    resolve(route: ActivatedRouteSnapshot): Observable<InnerGroups> {
        return this.groupService.getGroups(route.params['id']).pipe(
            catchError(error => {
                this.alertify.error('Problem retriving data');
                this.router.navigate(['/organizations']);
                return of(null);
            })
        );
    }
}
