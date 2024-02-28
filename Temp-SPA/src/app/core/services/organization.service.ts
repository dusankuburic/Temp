import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { InnerGroups } from '../models/group';
import { Organization } from '../models/organization';
import { Observable, map } from 'rxjs';
import { PaginatedResult } from '../models/pagination';

@Injectable({
  providedIn: 'root'
})
export class OrganizationService {
  baseUrl = environment.apiUrl;

constructor(private http: HttpClient) { }

getPagedOrganizations(page?, itemsPerPage?): Observable<PaginatedResult<Organization[]>> {
  const paginatedResult: PaginatedResult<Organization[]> = new PaginatedResult<Organization[]>();

  let params = new HttpParams();
  if (page != null && itemsPerPage != null) {
    params = params.append('pageNumber', page);
    params = params.append('pageSize', itemsPerPage);
  }

  return this.http.get<Organization[]>(this.baseUrl + 'organizations/paged-organizations', {observe: 'response', params})
    .pipe(
      map(response => {
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') != null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      })
    );
}

getOrganizations(): Observable<Organization[]> {
  return this.http.get<Organization[]>(this.baseUrl + 'organizations');
}

getOrganization(id: number): Observable<Organization> {
  return this.http.get<Organization>(this.baseUrl + 'organizations/' + id);
}

updateOrganization(organization: Organization): Observable<void> {
  return this.http.put<void>(this.baseUrl + 'organizations/' + organization.id, organization);
}

createOrganization(organization: Organization): Observable<Organization> {
  return this.http.post<Organization>(this.baseUrl + 'organizations', organization);
}

getInnerGroups(organizationId: number): Observable<InnerGroups> {
  return this.http.get<InnerGroups>(this.baseUrl + 'organizations/inner-groups/' + organizationId);
}

changeStatus(id: number): Observable<void> {
  return this.http.put<void>(this.baseUrl + 'organizations/change-status/' + id, {id});
}


}
