import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, ResolveFn, Router } from '@angular/router';
import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { PagedInnerGroups } from '../../models/group';
import { AlertifyService } from '../../services/alertify.service';
import { GroupService } from '../../services/group.service';

export const groupListResolver: ResolveFn<PagedInnerGroups | null> = (route: ActivatedRouteSnapshot) => {
    const groupService = inject(GroupService);
    const router = inject(Router);
    const alertify = inject(AlertifyService);

    const organizationId = parseInt(route.params['id']);
    groupService.resetGroupParams();
    return groupService.getInnerGroups(organizationId).pipe(
        catchError(() => {
            alertify.error('Unable to list groups');
            router.navigate(['']);
            return of(null);
        })
    );
};
