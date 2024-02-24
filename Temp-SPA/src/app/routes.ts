import { Routes } from '@angular/router';
import { UserGuard } from './core/guards/user.guard';
import { HomeComponent } from './home/home.component';
import { UsersComponent } from './users/users.component';
import { ModeratorGuard } from './core/guards/moderator.guard';
import { ModeratorComponent } from './moderator/moderator.component';

export const appRoutes: Routes = [
    {path: '', component: HomeComponent},
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [UserGuard],
        children: [
            {path: 'users', component: UsersComponent},
        ]
    },
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [ModeratorGuard],
        children: [
            {path: 'moderators', component: ModeratorComponent},
        ]
    },
    {path: '**', redirectTo: '', pathMatch: 'full'}
];
