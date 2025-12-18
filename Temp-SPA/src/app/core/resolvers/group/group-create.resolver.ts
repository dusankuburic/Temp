import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Organization } from '../../models/organization';
import { AlertifyService } from '../../services/alertify.service';
import { OrganizationService } from '../../services/organization.service';

@Injectable()
export class GroupCreateResolver implements Resolve<Organization> {

    constructor(
        private organizationService: OrganizationService,
        private router: Router,
        private alertify: AlertifyService
    ){}

    resolve(route: ActivatedRouteSnapshot): Observable<Organization>  {
        return this.organizationService.getOrganization(route.params['id']).pipe(
            catchError(() => {
                this.alertify.error('Unable to get Organization');
                this.router.navigate(['']);
                return of(null);
            })
        );
    }
}