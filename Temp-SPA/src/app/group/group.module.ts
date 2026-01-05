import { NgModule } from '@angular/core';
import { GroupListComponent } from './inner-group-list/inner-group-list.component';
import { GroupEditComponent } from './group-edit/group-edit.component';
import { GroupCreateComponent } from './group-create/group-create.component';
import { AssignedGroupsComponent } from './assigned-groups/assigned-groups.component';
import { RouterModule } from '@angular/router';
import { groupRoutes } from './group.routes';
import { SharedModule } from '../shared/shared.module';
import { GroupCreateModalComponent } from './group-create-modal/group-create-modal.component';
import { GroupEditModalComponent } from './group-edit-modal/group-edit-modal.component';
import { TabsModule } from 'ngx-bootstrap/tabs';

@NgModule({
    imports: [
        SharedModule,
        TabsModule,
        RouterModule.forChild(groupRoutes)
    ],
    exports: [],
    declarations: [
        GroupCreateComponent,
        GroupEditComponent,
        GroupListComponent,
        AssignedGroupsComponent,
        GroupCreateModalComponent,
        GroupEditModalComponent,
    ],
    providers: []
})
export class GroupModule { }
