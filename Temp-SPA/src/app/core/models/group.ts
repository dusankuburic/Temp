import { PaginatedResult } from "./pagination";

export interface Group {
    id: number;
    name: string;
    profilePictureUrl?: string;
    organizationId: number;
    hasActiveTeam: boolean;
}

export interface InnerGroups {
    id: number;
    name: string;
    groups: Group[];
}

export interface InnerGroup {
    id: number;
    name: string;
    hasActiveTeam: boolean;
}

export interface PagedInnerGroups {
    id: number;
    name: string;
    groups: PaginatedResult<Group[]>;
}

export interface ModeratorGroups {
    groups: number[];
}

export class GroupParams {
    pageNumber: number = 1;
    pageSize: number = 10;
    name: string = '';
    withTeams: string = 'all';
}
