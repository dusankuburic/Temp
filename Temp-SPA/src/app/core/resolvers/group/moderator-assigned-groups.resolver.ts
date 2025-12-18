import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, ResolveFn, Router } from '@angular/router';
import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Group } from 'src/app/core/models/group';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { GroupService } from 'src/app/core/services/group.service';

export const moderatorAssignedGroupsResolver: ResolveFn<Group[] | null> = (route: ActivatedRouteSnapshot) => {
    const groupService = inject(GroupService);
    const router = inject(Router);
    const alertify = inject(AlertifyService);

    return groupService.getModeratorGroups(route.params['id']).pipe(
        catchError(error => {
            alertify.error(error.error);
            router.navigate(['/']);
            return of(null);
        })
    );
};
