import { NgModule } from '@angular/core';
import { OrganizationCreateComponent } from './organization-create/organization-create.component';
import { OrganizationEditComponent } from './organization-edit/organization-edit.component';
import { OrganizationListComponent } from './organization-list/organization-list.component';
import { RouterModule } from '@angular/router';
import { organizationRoutes } from './organization.routes';
import { SharedModule } from '../shared/shared.module';
import { OrganizationListResolver } from '../core/resolvers/organization/organization-list.resolver';
import { OrganizationEditResolver } from '../core/resolvers/organization/organization-edit.resolver';

@NgModule({
    imports: [
        SharedModule,
        RouterModule.forChild(organizationRoutes)
    ],
    exports: [],
    declarations: [
        OrganizationCreateComponent,
        OrganizationEditComponent,
        OrganizationListComponent,
    ],
    providers: [
        OrganizationListResolver,
        OrganizationEditResolver,
    ]
})
export class OrganizationModule { }
