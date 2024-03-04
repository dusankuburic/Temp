import { Routes } from "@angular/router";
import { AuthGuard } from "../core/guards/auth.guard";
import { GroupListComponent } from "./inner-group-list/inner-group-list.component";
import { GroupListResolver } from "../core/resolvers/group/group-list.resolver";
import { GroupCreateComponent } from "./group-create/group-create.component";
import { GroupCreateResolver } from "../core/resolvers/group/group-create.resolver";
import { GroupEditComponent } from "./group-edit/group-edit.component";
import { GroupEditResolver } from "../core/resolvers/group/group-edit.resolver";
import { ModeratorGuard } from "../core/guards/moderator.guard";
import { AssignedGroupsComponent } from "./assigned-groups/assigned-groups.component";
import { ModeratorAssignedGroupsResolver } from "../core/resolvers/group/moderator-assigned-groups.resolver";

export const groupRoutes: Routes = [    
    { 
        canActivate: [AuthGuard],
        path: 'organization/:id', 
        component: GroupListComponent, 
        resolve: { innergroups: GroupListResolver }
    },
    { 
        canActivate: [AuthGuard],
        path: 'create/organization/:id', 
        component: GroupCreateComponent, 
        resolve: { organization: GroupCreateResolver }
    },
    { 
        canActivate: [AuthGuard],
        path: ':id/edit/:organizationId/organization', 
        component: GroupEditComponent, 
        resolve: { group: GroupEditResolver }
    },
    {
        canActivate: [ModeratorGuard],
        path: ':id/assigned-groups',
        component: AssignedGroupsComponent,
        resolve: { groups: ModeratorAssignedGroupsResolver }
    }
];