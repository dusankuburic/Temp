
import { NgModule } from '@angular/core';
import { WorkplaceCreateComponent } from './workplace-create/workplace-create.component';
import { WorkplaceEditComponent } from './workplace-edit/workplace-edit.component';
import { WorkplaceListComponent } from './workplace-list/workplace-list.component';
import { CommonModule } from '@angular/common';
import { AppRoutingModule } from '../app-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { RouterModule } from '@angular/router';
import { workplaceRoutes } from './workplace.routes';

@NgModule({
    imports: [
        CommonModule,
        AppRoutingModule,
        ReactiveFormsModule,
        PaginationModule.forRoot(),
        FormsModule,
        FontAwesomeModule,
        RouterModule.forChild(workplaceRoutes)
    ],
    exports: [],
    declarations: [
        WorkplaceCreateComponent,
        WorkplaceEditComponent,
        WorkplaceListComponent,
    ],
})
export class WorkplaceModule { }
