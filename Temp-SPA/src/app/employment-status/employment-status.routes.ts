import { Routes } from "@angular/router";
import { EmploymentStatusListComponent } from "./employment-status-list/employment-status-list.component";
import { EmploymentStatusListResolver } from "../core/resolvers/employment-status/employment-status-list.resolver";
import { EmploymentStatusEditComponent } from "./employment-status-edit/employment-status-edit.component";
import { EmploymentStatusEditResolver } from "../core/resolvers/employment-status/employment-status-edit.resolver";
import { EmploymentStatusCreateComponent } from "./employment-status-create/employment-status-create.component";

export const employmentStatusRoutes: Routes = [    
    { 
        path: '',
        component: EmploymentStatusListComponent,
        resolve: { employmentStatuses: EmploymentStatusListResolver }
    },
    { 
        path: ':id/edit',
        component: EmploymentStatusEditComponent,
        resolve: { employmentStatus: EmploymentStatusEditResolver }
    },
    {
        path: 'create',
        component: EmploymentStatusCreateComponent
    }
];