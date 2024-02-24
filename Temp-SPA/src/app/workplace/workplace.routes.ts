import { Routes } from "@angular/router";
import { WorkplaceListComponent } from "./workplace-list/workplace-list.component";
import { WorkplaceListResolver } from "../core/resolvers/workplace/workplace-list.resolver";
import { WorkplaceEditComponent } from "./workplace-edit/workplace-edit.component";
import { WorkplaceEditResolver } from "../core/resolvers/workplace/workplace-edit.resolver";
import { WorkplaceCreateComponent } from "./workplace-create/workplace-create.component";

export const workplaceRoutes: Routes = [    
    { 
        path: '', 
        component: WorkplaceListComponent, 
        resolve: { workplaces: WorkplaceListResolver }
    },
    {
        path: ':id/edit',
        component: WorkplaceEditComponent,
        resolve: { workplace: WorkplaceEditResolver }
    },
    {
        path: 'create',
        component: WorkplaceCreateComponent
    }  
];