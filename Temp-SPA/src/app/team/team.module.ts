import { NgModule } from '@angular/core';
import { TeamListComponent } from './inner-team-list/inner-team-list.component';
import { TeamCreateComponent } from './team-create/team-create.component';
import { TeamEditComponent } from './team-edit/team-edit.component';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from '../app-routing.module';
import { AssignedInnerTeamsComponent } from './assigned-inner-teams/assigned-inner-teams.component';


@NgModule({
    imports: [
        CommonModule,
        ReactiveFormsModule,
        AppRoutingModule,        
    ],
    exports: [],
    declarations: [
        TeamListComponent,
        TeamCreateComponent,
        TeamEditComponent,
        AssignedInnerTeamsComponent,
    ],
})
export class TeamModule { }