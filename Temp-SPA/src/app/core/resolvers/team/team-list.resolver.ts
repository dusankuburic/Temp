import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Team } from '../../models/team';
import { AlertifyService } from '../../services/alertify.service';
import { TeamService } from '../../services/team.service';

@Injectable()
export class TeamListResolver  {

    constructor(
        private teamService: TeamService,
        private router: Router,
        private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<Team[]>  {
        return this.teamService.getTeams(route.params['id']).pipe(
            catchError(error => {    
                this.alertify.error('Problem retrieving data');
                //this.router.navigate(['/organizations']);
                return of(null);
            })
        );
    }
}