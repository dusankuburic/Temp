import { Routes } from "@angular/router";
import { AuthGuard } from "../core/guards/auth.guard";
import { EngagementWithoutEmployeeListComponent } from "./engagement-without-employee-list/engagement-without-employee-list.component";
import { EngagementWithoutEmployeeResolver } from "../core/resolvers/engagement/engagement-without-employee-list.resolver";
import { EngagementCreateComponent } from "./engagement-create/engagement-create.component";
import { EngagementCreateResolver } from "../core/resolvers/engagement/engagement-create.resolver";
import { EngagementWithEmployeeListComponent } from "./engagement-with-employee-list/engagement-with-employee-list.component";
import { EngagementWithEmployeeResolver } from "../core/resolvers/engagement/engagement-with-employee-list.resolver";
import { UserGuard } from "../core/guards/user.guard";
import { EngagementUserListComponent } from "./engagement-user-list/engagement-user-list.component";
import { EngagementUserListResolver } from "../core/resolvers/engagement/engagement-user-list.resolver";

export const engagementRoutes: Routes = [
    { 
        canActivate: [AuthGuard],
        path: '', component: EngagementWithoutEmployeeListComponent, 
        resolve: { employeesWithout: EngagementWithoutEmployeeResolver }
    },
    { 
        canActivate: [AuthGuard],
        path: 'create/:id',
        component: EngagementCreateComponent,
        resolve: { employeeData: EngagementCreateResolver }
    },
    {
        canActivate: [AuthGuard],
        path: 'with-employee',
        component: EngagementWithEmployeeListComponent,
        resolve: { employeesWith: EngagementWithEmployeeResolver }
    },
    {
        canActivate: [AuthGuard],
        path: 'without-employee',
        component: EngagementWithoutEmployeeListComponent,
        resolve: {employeesWithout: EngagementWithoutEmployeeResolver}
    },
    {
        canActivate: [UserGuard],
        path: 'user-list/:id',
        component: EngagementUserListComponent,
        resolve: { engagements: EngagementUserListResolver },
    }
];