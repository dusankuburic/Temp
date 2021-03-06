import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Group } from '../../_models/group';
import { AlertifyService } from '../../_services/alertify.service';
import { GroupService } from '../../_services/group.service';

@Injectable({ providedIn: 'root' })
export class GroupEditResolver implements Resolve<Group> {

    constructor(
        private groupService: GroupService,
        private router: Router,
        private alertify: AlertifyService){}

    resolve(route: ActivatedRouteSnapshot): Observable<Group> {
        return this.groupService.getGroup(route.params['id']).pipe(
            catchError(error => {
                this.alertify.error('Problem retriving data');
                this.router.navigate(['/organizations']);
                return of(null);
            })
        );
    }
}