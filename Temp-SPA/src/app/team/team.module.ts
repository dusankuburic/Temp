import { NgModule } from '@angular/core';
import { TeamListComponent } from './inner-team-list/inner-team-list.component';
import { TeamCreateComponent } from './team-create/team-create.component';
import { TeamEditComponent } from './team-edit/team-edit.component';
import { AssignedInnerTeamsComponent } from './assigned-inner-teams/assigned-inner-teams.component';
import { RouterModule } from '@angular/router';
import { teamRoutes } from './team.routes';
import { SharedModule } from '../shared/shared.module';
import { TeamValidators } from './team-validators';
import { TeamCreateModalComponent } from './team-create-modal/team-create-modal.component';
import { TeamEditModalComponent } from './team-edit-modal/team-edit-modal.component';

@NgModule({
    imports: [
        SharedModule,      
        RouterModule.forChild(teamRoutes)
    ],
    exports: [],
    declarations: [
        TeamListComponent,
        TeamCreateComponent,
        TeamEditComponent,
        AssignedInnerTeamsComponent,
        TeamCreateModalComponent,
        TeamEditModalComponent,
    ],
    providers: [
        TeamValidators,
    ]
})
export class TeamModule { }
