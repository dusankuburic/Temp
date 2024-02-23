import { NgModule } from '@angular/core';
import { EngagementCreateComponent } from './engagement-create/engagement-create.component';
import { EngagementUserListComponent } from './engagement-user-list/engagement-user-list.component';
import { EngagementWithEmployeeListComponent } from './engagement-with-employee-list/engagement-with-employee-list.component';
import { EngagementWithoutEmployeeListComponent } from './engagement-without-employee-list/engagement-without-employee-list.component';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from '../app-routing.module';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';



@NgModule({
    imports: [
        CommonModule,
        ReactiveFormsModule,
        AppRoutingModule,
        PaginationModule.forRoot(),
        BsDatepickerModule.forRoot(),
        FontAwesomeModule,
        FormsModule,
    ],
    exports: [],
    declarations: [
        EngagementCreateComponent,
        EngagementUserListComponent,
        EngagementWithEmployeeListComponent,
        EngagementWithoutEmployeeListComponent,
    ],
})
export class EngagementModule { }
