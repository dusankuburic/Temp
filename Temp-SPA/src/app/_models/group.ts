export interface Group {
    id: number;
    name: string;
}

export interface InnerGroups {
    name: string;
    groups: Group[];
}
