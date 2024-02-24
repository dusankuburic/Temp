import { NgModule } from '@angular/core';
import { OrganizationCreateComponent } from './organization-create/organization-create.component';
import { OrganizationEditComponent } from './organization-edit/organization-edit.component';
import { OrganizationListComponent } from './organization-list/organization-list.component';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from '../app-routing.module';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { RouterModule } from '@angular/router';
import { organizationRoutes } from './organization.routes';

@NgModule({
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FontAwesomeModule,
        AppRoutingModule,
        RouterModule.forChild(organizationRoutes)
    ],
    exports: [],
    declarations: [
        OrganizationCreateComponent,
        OrganizationEditComponent,
        OrganizationListComponent,
    ],
})
export class OrganizationModule { }
