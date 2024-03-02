export interface Organization {
    id: number;
    name: string;
}

export class OrganizationParams {
    pageNumber: number = 1;
    pageSize: number = 5;
    name: string = '';
}