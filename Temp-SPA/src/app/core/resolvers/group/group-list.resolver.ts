import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { PagedInnerGroups } from '../../models/group';
import { AlertifyService } from '../../services/alertify.service';
import { GroupService } from '../../services/group.service';

@Injectable()
export class GroupListResolver  {
    constructor(
        private groupService: GroupService,
        private router: Router,
        private alertify: AlertifyService){}

    resolve(route: ActivatedRouteSnapshot): Observable<PagedInnerGroups> {
        const organizationId = parseInt(route.params['id']);
        this.groupService.resetGroupParams();
        return this.groupService.getInnerGroups(organizationId).pipe(
            catchError(error => {
                this.alertify.error('Unable to list groups');
                this.router.navigate(['']);
                return of(null);
            })
        )
    }
}
