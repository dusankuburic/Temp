import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Organization } from '../../_models/organization';
import { AlertifyService } from '../../_services/alertify.service';
import { OrganizationService } from '../../_services/organization.service';

@Injectable()
export class OrganizationListResolver  {

    constructor(
        private organizationService: OrganizationService,
        private router: Router,
        private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<Organization[]>{
        return this.organizationService.getOrganizations().pipe(
            catchError(error => {
                this.alertify.error('Problem retriving data');
                this.router.navigate(['']);
                return of(null);
            })
        );
    }
}