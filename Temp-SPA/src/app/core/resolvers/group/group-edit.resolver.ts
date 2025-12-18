import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Group } from '../../models/group';
import { AlertifyService } from '../../services/alertify.service';
import { GroupService } from '../../services/group.service';

@Injectable()
export class GroupEditResolver implements Resolve<Group> {

    constructor(
        private groupService: GroupService,
        private router: Router,
        private alertify: AlertifyService){}

    resolve(route: ActivatedRouteSnapshot): Observable<Group> {
        return this.groupService.getGroup(route.params['id']).pipe(
            catchError(() => {
                this.alertify.error('Unable to get Group');
                this.router.navigate(['/organizations']);
                return of(null);
            })
        );
    }
}