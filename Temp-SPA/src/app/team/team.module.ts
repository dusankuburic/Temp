import { NgModule } from '@angular/core';
import { TeamListComponent } from './inner-team-list/inner-team-list.component';
import { TeamCreateComponent } from './team-create/team-create.component';
import { TeamEditComponent } from './team-edit/team-edit.component';
import { AssignedInnerTeamsComponent } from './assigned-inner-teams/assigned-inner-teams.component';
import { RouterModule } from '@angular/router';
import { teamRoutes } from './team.routes';
import { SharedModule } from '../shared/shared.module';
import { TeamListResolver } from '../core/resolvers/team/team-list.resolver';
import { TeamCreateResolver } from '../core/resolvers/team/team-create.resolver';
import { TeamEditResolver } from '../core/resolvers/team/team-edit.resolver';

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
    ],
    providers: [
        TeamListResolver,
        TeamCreateResolver,
        TeamEditResolver,
    ]
})
export class TeamModule { }
