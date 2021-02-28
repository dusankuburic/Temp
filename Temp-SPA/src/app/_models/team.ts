export interface Team {
    id: number;
    name: string;
    groupId: number;
}

export interface InnerTeams {
    name: string;
    teams: Team[];
}