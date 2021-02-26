export interface Group {
    id: number;
    name: string;
    organizationId: number;
}

export interface InnerGroups {
    name: string;
    groups: Group[];
}
