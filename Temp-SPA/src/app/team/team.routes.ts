import { Routes } from "@angular/router";
import { AuthGuard } from "../core/guards/auth.guard";
import { TeamListComponent } from "./inner-team-list/inner-team-list.component";
import { TeamListResolver } from "../core/resolvers/team/team-list.resolver";
import { TeamCreateComponent } from "./team-create/team-create.component";
import { TeamCreateResolver } from "../core/resolvers/team/team-create.resolver";
import { TeamEditComponent } from "./team-edit/team-edit.component";
import { TeamEditResolver } from "../core/resolvers/team/team-edit.resolver";
import { ModeratorGuard } from "../core/guards/moderator.guard";
import { AssignedInnerTeamsComponent } from "./assigned-inner-teams/assigned-inner-teams.component";

export const teamRoutes: Routes = [
    {
        canActivate: [AuthGuard],
        path: 'group/:id',
        component: TeamListComponent,
        resolve: { innerteams: TeamListResolver }
    },
    {
        canActivate: [AuthGuard],
        path: 'create/group/:id',
        component: TeamCreateComponent,
        resolve: { group: TeamCreateResolver }
    },
    {
        canActivate: [AuthGuard],
        path: ':id/edit/:groupId/group',
        component: TeamEditComponent,
        resolve: { team: TeamEditResolver}
    },
    { 
        canActivate: [ModeratorGuard],
        path: ':id/assigned-groups',
        component: AssignedInnerTeamsComponent,
        resolve: { teams: TeamListResolver }
    }
];