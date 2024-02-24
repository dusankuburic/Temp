import { Routes } from "@angular/router";
import { AuthGuard } from "../core/guards/auth.guard";
import { WorkplaceListComponent } from "./workplace-list/workplace-list.component";
import { WorkplaceListResolver } from "../core/resolvers/workplace/workplace-list.resolver";
import { WorkplaceEditComponent } from "./workplace-edit/workplace-edit.component";
import { WorkplaceEditResolver } from "../core/resolvers/workplace/workplace-edit.resolver";
import { WorkplaceCreateComponent } from "./workplace-create/workplace-create.component";

export const workplaceRoutes: Routes = [
    {
        path: 'workplaces',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            { path: '', component: WorkplaceListComponent, resolve: { workplaces: WorkplaceListResolver}},
            {
                path: ':id/edit',
                component: WorkplaceEditComponent,
                resolve: { workplace: WorkplaceEditResolver }
            },
            {
                path: 'create',
                component: WorkplaceCreateComponent
            }
        ]
    }
];