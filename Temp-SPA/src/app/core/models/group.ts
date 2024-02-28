import { PaginatedResult } from "./pagination";

export interface Group {
    id: number;
    name: string;
    organizationId: number;
}

export interface InnerGroups {
    id: number;
    name: string;
    groups: Group[];
}

export interface InnerGroup {
    id: number;
    name: string;
}

export interface PagedInnerGroups {
    id: number;
    name: string;
    groups: PaginatedResult<Group[]>;
}

export interface ModeratorGroups {
    groups: number[];
}
