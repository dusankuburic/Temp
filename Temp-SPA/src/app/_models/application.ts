export interface Application {
    id: number;
    userId: number;
    teamId: number;
    moderatorId?: number;
    category: string;
    content: string;
}


export interface CreateApplication{
    userId: number;
    teamId: number;
    content: string;
    category: string;
}