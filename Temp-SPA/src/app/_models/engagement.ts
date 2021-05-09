export interface Engagement {
    id: number;
    employeeId: number;
    workplaceId: number;
    employmentStatusId: number;
    dateFrom: Date;
    dateTo: Date;
}

export interface UserEngagement {
    workplaceName: string;
    employmentStatusName: string;
    dateFrom: Date;
    dateTo: Date;
    salary: number;
}