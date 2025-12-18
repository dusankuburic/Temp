import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Team } from 'src/app/core/models/team';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { TeamService } from 'src/app/core/services/team.service';

@Injectable()
export class ApplicationCreateResolver implements Resolve<Team> {

    constructor(
        private teamService: TeamService,
        private router: Router,
        private alertify: AlertifyService
    ){}

    resolve(route: ActivatedRouteSnapshot): Observable<Team> {
        return  this.teamService.getUserTeam(route.params['id']).pipe(
            catchError(() => {
                this.alertify.error('Unable to get user team');
                this.router.navigate(['']);
                return of(null);
            })
        );
    }
}