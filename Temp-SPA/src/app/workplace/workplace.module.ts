
import { NgModule } from '@angular/core';
import { WorkplaceCreateComponent } from './workplace-create/workplace-create.component';
import { WorkplaceEditComponent } from './workplace-edit/workplace-edit.component';
import { WorkplaceListComponent } from './workplace-list/workplace-list.component';
import { RouterModule } from '@angular/router';
import { workplaceRoutes } from './workplace.routes';
import { SharedModule } from '../shared/shared.module';
import { WorkplaceListResolver } from '../core/resolvers/workplace/workplace-list.resolver';
import { WorkplaceEditResolver } from '../core/resolvers/workplace/workplace-edit.resolver';
import { WorkplaceValidators } from './workplace-validators';

@NgModule({
    imports: [
        SharedModule,
        RouterModule.forChild(workplaceRoutes)
    ],
    exports: [],
    declarations: [
        WorkplaceCreateComponent,
        WorkplaceEditComponent,
        WorkplaceListComponent,
    ],
    providers: [
        WorkplaceValidators,
        WorkplaceListResolver,
        WorkplaceEditResolver,
    ]
})
export class WorkplaceModule { }
