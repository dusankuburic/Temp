export interface Organization {
    id: number;
    name: string;
}

export type UpdateOrganizationStatus = Pick<Organization, "id">;