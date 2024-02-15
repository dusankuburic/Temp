export interface Application {
    id: number;
    category: string;
    content: string;
    createdAt: Date;
}

export interface CreateApplication{
    userId: number;
    teamId: number;
    content: string;
    category: string;
}

export interface ModeratorListApplication {
    id: number;
    teamId: number;
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

export interface UpdateApplicationRequest {
    id: number;
    moderatorId: number;
}
