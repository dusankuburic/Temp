
export interface Employee {
    id: number;
    firstName: string;
    lastName: string;
    role: string;
    teamId: number;
    isActive: boolean;
}

export class EmployeeParams {
    pageNumber: number = 1;
    pageSize: number = 5;
    role: string = '';
    firstName: string = '';
    lastName: string = '';
}