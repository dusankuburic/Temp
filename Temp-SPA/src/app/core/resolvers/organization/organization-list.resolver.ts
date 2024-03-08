import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Organization } from '../../models/organization';
import { AlertifyService } from '../../services/alertify.service';
import { OrganizationService } from '../../services/organization.service';
import { PaginatedResult } from '../../models/pagination';

@Injectable()
export class OrganizationListResolver  {
    constructor(
        private organizationService: OrganizationService,
        private router: Router,
        private alertify: AlertifyService) {}
    
    resolve(route: ActivatedRouteSnapshot): Observable<PaginatedResult<Organization[]>> {
        this.organizationService.resetOrganizationParams();
        return this.organizationService.getPagedOrganizations().pipe(
            catchError(error => {
                this.alertify.error('Unable to list Organizations');
                this.router.navigate(['']);
                return of(null);
            })
        )
    }
}