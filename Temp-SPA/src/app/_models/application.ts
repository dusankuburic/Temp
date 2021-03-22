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

export interface ModeratorListApplication {
    id: number;
    username: string;
    category: string;
    createdAt: Date;
    status: boolean;
}

export interface UserListApplication {
    id: number;
    category: string;
    createdAt: Date;
    status: boolean;
}