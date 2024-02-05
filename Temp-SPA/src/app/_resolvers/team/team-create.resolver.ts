import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Group } from '../../_models/group';
import { AlertifyService } from '../../_services/alertify.service';
import { GroupService } from '../../_services/group.service';

@Injectable()
export class TeamCreateResolver  {

    constructor(
        private groupService: GroupService,
        private router: Router,
        private alertify: AlertifyService
    ){}

    resolve(route: ActivatedRouteSnapshot): Observable<Group> {
        return this.groupService.getGroup(route.params['id']).pipe(
            catchError(error => {
                this.alertify.error('Problem retriving data');
                this.router.navigate(['']);
                return of(null);
            })
        );
    }
}