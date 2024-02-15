import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Group } from '../models/group';
import { Organization, UpdateOrganizationStatus } from '../models/organization';

@Injectable({
  providedIn: 'root'
})
export class OrganizationService {
  baseUrl = environment.apiUrl;

constructor(private http: HttpClient) { }


getOrganizations(): any {
  return this.http.get<Organization[]>(this.baseUrl + 'organizations');
}

getOrganization(id: number): any {
  return this.http.get<Organization>(this.baseUrl + 'organizations/' + id);
}

updateOrganization(organization: Organization): any {
  return this.http.put(this.baseUrl + 'organizations/', organization);
}

createOrganization(organization: Organization): any {
  return this.http.post(this.baseUrl + 'organizations', organization);
}

getInnerGroups(organizationId: number): any {
  return this.http.get<Group[]>(this.baseUrl + 'organizations/inner-groups/' + organizationId);
}

changeStatus(request: UpdateOrganizationStatus): any {
  return this.http.put(this.baseUrl + 'organizations/change-status', request);
}


}
