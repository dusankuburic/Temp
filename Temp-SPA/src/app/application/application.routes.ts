import { Routes } from "@angular/router";
import { UserGuard } from "../core/guards/user.guard";
import { ApplicationCreateComponent } from "./application-create/application-create.component";
import { ApplicationCreateResolver } from "../core/resolvers/application/application-create.resolver";
import { ApplicationUserListComponent } from "./application-user-list/application-user-list.component";
import { ApplicationUserListResolver } from "../core/resolvers/application/application-user-list.resolver";
import { ApplicationUserComponent } from "./application-user/application-user.component";
import { ApplicationUserResolver } from "../core/resolvers/application/application-user.resolver";
import { ModeratorGuard } from "../core/guards/moderator.guard";
import { ApplicationModeratorListComponent } from "./application-moderator-list/application-moderator-list.component";
import { ApplicationModeratorListResolver } from "../core/resolvers/application/application-moderator-list.resolver";
import { ApplicationModeratorComponent } from "./application-moderator/application-moderator.component";
import { ApplicationModeratorResolver } from "../core/resolvers/application/application-moderator.resolver";

export const applicationRoutes: Routes = [
    { 
        canActivate: [UserGuard],
        path: 'create/:id', 
        component: ApplicationCreateComponent, 
        resolve: { team: ApplicationCreateResolver }
    },
    { 
        canActivate: [UserGuard],
        path: 'list/:id',
        component: ApplicationUserListComponent,
        resolve: { applications: ApplicationUserListResolver }
    },
    {
        canActivate: [UserGuard],
        path: 'user/:id',
        component: ApplicationUserComponent,
        resolve: { application: ApplicationUserResolver }
    },
    {
        canActivate: [ModeratorGuard],
        path: 'list/team/:id',
        component: ApplicationModeratorListComponent,
        resolve: { applications: ApplicationModeratorListResolver }
    },
    {
        canActivate: [ModeratorGuard],
        path: 'moderator/:id',
        component: ApplicationModeratorComponent,
        resolve: { application: ApplicationModeratorResolver }
    } 
];