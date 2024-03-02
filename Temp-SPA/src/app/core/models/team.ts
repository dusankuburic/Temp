import { PaginatedResult } from "./pagination";

export interface Team {
    id: number;
    name: string;
    groupId: number;
}

export interface InnerTeams {
    id: number;
    name: string;
    teams: Team[];
}

export interface InnerTeam {
    id: number;
    name: string;
}

export interface PagedInnerTeams {
    id: number;
    name: string;
    teams: PaginatedResult<Team[]>;
}

export interface FullTeam {
    id: number;
    organizationName: string;
    organizationId: number;
    groupName: string;
    teamName: string;
}

export class TeamParams {
    pageNumber: number = 1;
    pageSize: number = 5;
    name: string = '';
}