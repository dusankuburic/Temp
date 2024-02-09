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
    organizationId: number;
    groupName: string;
    teamName: string;
}