
export interface Workplace {
    id: number;
    name: string;
}

export interface UpdateWorkplaceStatus {
    id: number;
}

export class WorkplaceParams {
    pageNumber: number = 1;
    pageSize: number = 5;
    name: string = '';
}