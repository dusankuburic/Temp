import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { PagedInnerTeams, Team } from '../../models/team';
import { AlertifyService } from '../../services/alertify.service';
import { TeamService } from '../../services/team.service';

@Injectable()
export class TeamListResolver  {

    pageNumber = 1;
    pageSize = 5;

    constructor(
        private teamService: TeamService,
        private router: Router,
        private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<PagedInnerTeams>  {
        const groupId = parseInt(route.params['id']);
        return this.teamService.getInnerTeams(this.pageNumber, this.pageSize, groupId).pipe(
            catchError(error => {
                this.alertify.error('Problem retrieving teams');
                this.router.navigate(['']);
                return of(null);
            })
        );
    }
}