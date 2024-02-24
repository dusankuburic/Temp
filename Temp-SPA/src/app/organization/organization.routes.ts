import { Routes } from "@angular/router";
import { AuthGuard } from "../core/guards/auth.guard";
import { OrganizationListComponent } from "./organization-list/organization-list.component";
import { OrganizationListResolver } from "../core/resolvers/organization/organization-list.resolver";
import { OrganizationEditComponent } from "./organization-edit/organization-edit.component";
import { OrganizationEditResolver } from "../core/resolvers/organization/organization-edit.resolver";
import { OrganizationCreateComponent } from "./organization-create/organization-create.component";

export const organizationRoutes: Routes = [
    {
        path: 'organizations',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            { path: '', component: OrganizationListComponent, resolve: { organizations: OrganizationListResolver }},
            {
                path: ':id/edit',
                component: OrganizationEditComponent,
                resolve: { organization: OrganizationEditResolver }
            }, 
            {
                path: 'create',
                component: OrganizationCreateComponent
            }
        ]
    }
];