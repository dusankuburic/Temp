import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { InnerTeams } from '../../_models/team';
import { AlertifyService } from '../../_services/alertify.service';
import { TeamService } from '../../_services/team.service';

@Injectable()
export class TeamListResolver implements Resolve<InnerTeams> {

    constructor(
        private teamService: TeamService,
        private router: Router,
        private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<InnerTeams>  {
        return this.teamService.getTeams(route.params['id']).pipe(
            catchError(error => {    
                this.alertify.error('Problem retriving data');
                //this.router.navigate(['/organizations']);
                return of(null);
            })
        );
    }
}