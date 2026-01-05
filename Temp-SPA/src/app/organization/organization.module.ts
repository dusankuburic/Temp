import { NgModule } from '@angular/core';
import { OrganizationCreateComponent } from './organization-create/organization-create.component';
import { OrganizationEditComponent } from './organization-edit/organization-edit.component';
import { OrganizationListComponent } from './organization-list/organization-list.component';
import { RouterModule } from '@angular/router';
import { organizationRoutes } from './organization.routes';
import { SharedModule } from '../shared/shared.module';
import { OrganizationCreateModalComponent } from './organization-create-modal/organization-create-modal.component';
import { OrganizationEditModalComponent } from './organization-edit-modal/organization-edit-modal.component';
import { TabsModule } from 'ngx-bootstrap/tabs';

@NgModule({
    imports: [
        SharedModule,
        TabsModule,
        RouterModule.forChild(organizationRoutes)
    ],
    exports: [],
    declarations: [
        OrganizationCreateComponent,
        OrganizationEditComponent,
        OrganizationListComponent,
        OrganizationCreateModalComponent,
        OrganizationEditModalComponent,
    ],
    providers: []
})
export class OrganizationModule { }
