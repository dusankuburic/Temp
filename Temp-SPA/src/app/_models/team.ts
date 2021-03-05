export interface Team {
    id: number;
    name: string;
    groupId: number;
}

export interface InnerTeams {
    name: string;
    teams: Team[];
}

export interface FullTeam {
    id: number;
    organizationName: string;
    groupName: string;
    teamName: string;
}