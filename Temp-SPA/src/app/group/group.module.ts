import { NgModule } from '@angular/core';
import { GroupListComponent } from './inner-group-list/inner-group-list.component';
import { GroupEditComponent } from './group-edit/group-edit.component';
import { GroupCreateComponent } from './group-create/group-create.component';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from '../app-routing.module';
import { AssignedGroupsComponent } from './assigned-groups/assigned-groups.component';

@NgModule({
    imports: [
        CommonModule,
        ReactiveFormsModule,
        AppRoutingModule,
    ],
    exports: [],
    declarations: [
        GroupCreateComponent,
        GroupEditComponent,
        GroupListComponent,
        AssignedGroupsComponent,
    ],
})
export class GroupModule { }
