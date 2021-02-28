export interface Team {
    id: number;
    name: string;
    teamId: number;
}

export interface InnerTeams {
    name: string;
    teams: Team[];
}