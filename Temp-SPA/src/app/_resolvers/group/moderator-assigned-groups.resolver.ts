import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Group } from 'src/app/_models/group';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { GroupService } from 'src/app/_services/group.service';

@Injectable()
export class ModeratorAssignedGroupsResolver  {

    constructor(
        private groupService: GroupService,
        private router: Router,
        private alertify: AlertifyService){}

    resolve(route: ActivatedRouteSnapshot): Observable<Group[]> {
        return this.groupService.getModeratorGroups(route.params['id']).pipe(
            catchError(error => {
                this.alertify.error(error.error);
                this.router.navigate(['/']);
                return of(null);
            })
        );
    }
}
