import { Routes } from '@angular/router';
import { userGuard } from './core/guards/user.guard';
import { HomeComponent } from './home/home.component';
import { UsersComponent } from './user/users/users.component';
import { moderatorGuard } from './core/guards/moderator.guard';
import { ModeratorComponent } from './user/moderator/moderator.component';
import { authGuard } from './core/guards/auth.guard';

export const appRoutes: Routes = [
    {
        path: 'employees',
        canActivate: [authGuard],
        loadChildren: () => import('./employee/employee.module')
            .then(m => m.EmployeeModule)
    },
    {
        path: 'employment-statuses',
        canActivate: [authGuard],
        loadChildren: () => import('./employment-status/employment-status.module')
            .then(m => m.EmploymentStatusModule)
    },
    {
        path: 'organizations',
        canActivate: [authGuard],
        loadChildren: () => import('./organization/organization.module')
            .then(m => m.OrganizationModule)
    },
    {
        path: 'workplaces',
        canActivate: [authGuard],
        loadChildren: () => import('./workplace/workplace.module')
            .then(m => m.WorkplaceModule)
    },
    {
        path: 'engagements',
        loadChildren: () => import('./engagement/engagement.module')
            .then(m => m.EngagementModule)
    },
    {
        path: 'groups',
        loadChildren: () => import('./group/group.module')
            .then(m => m.GroupModule)
    },
    {
        path: 'applications',
        loadChildren: () => import('./application/application.module')
            .then(m => m.ApplicationModule)
    },
    {
        path: 'teams',
        loadChildren: () => import('./team/team.module')
            .then(m => m.TeamModule)
    },
    {path: '', component: HomeComponent},
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [userGuard],
        children: [
            {path: 'users', component: UsersComponent},
        ]
    },
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [moderatorGuard],
        children: [
            {path: 'moderators', component: ModeratorComponent},
        ]
    },
    {path: '**', redirectTo: '', pathMatch: 'full'}
];
