import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Application } from 'src/app/core/models/application';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { ApplicationService } from 'src/app/core/services/application.service';

@Injectable()
export class ApplicationUserResolver implements Resolve<Application | null> {

    constructor(
        private applicationService: ApplicationService,
        private router: Router,
        private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<Application | null> {
        return this.applicationService.getApplication(route.params['id']).pipe(
            catchError(() => {
                this.alertify.error('Unable to get Application');
                this.router.navigate(['']);
                return of(null);
            })
        );
    }
}