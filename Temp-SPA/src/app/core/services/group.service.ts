import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Group, GroupParams, InnerGroups, PagedInnerGroups } from '../models/group';
import { ModeratorMin } from '../models/moderator';
import { InnerTeams } from '../models/team';
import { Observable, map } from 'rxjs';
import { PaginatedResult } from '../models/pagination';

@Injectable({
  providedIn: 'root'
})
export class GroupService {
  baseUrl = environment.apiUrl;
  groupParams = new GroupParams();

constructor(private http: HttpClient) { }

setGroupParams(params: GroupParams) {
  this.groupParams = params;
}

getGroupParams(): GroupParams {
  return this.groupParams;
}

resetGroupParams(): void {
  this.groupParams.pageNumber = 1;
  this.groupParams.pageSize = 5;
  this.groupParams.name = '';
}

getInnerGroups(organizationId: number): Observable<PagedInnerGroups> {
  const paginatedResult: PaginatedResult<Group[]> = new PaginatedResult<Group[]>();

  let params = new HttpParams();

  params = params.append('organizationId', organizationId);
  params = params.append('pageNumber', this.groupParams.pageNumber);
  params = params.append('pageSize', this.groupParams.pageSize);

  if (this.groupParams.name) {
    params = params.append('name', this.groupParams.name);
  }

  return this.http.get<InnerGroups>(this.baseUrl + 'organizations/paged-inner-groups', {observe: 'response', params})
    .pipe(
      map(response => {
        paginatedResult.result = response.body.groups;
        if (response.headers.get('Pagination') != null) {
          paginatedResult.pagination =  JSON.parse(response.headers.get('Pagination'));
        }
        return {
          id: response.body.id,
          name: response.body.name,
          groups: paginatedResult
        };
      })
    );
}

getGroups(organizationId: number): Observable<InnerGroups> {
  return this.http.get<InnerGroups>(this.baseUrl + 'organizations/inner-groups/' + organizationId);
}

getGroup(id: number): Observable<Group> {
  return this.http.get<Group>(this.baseUrl + 'groups/' + id);
}


createGroup(group: Group): Observable<Group> {
  return this.http.post<Group>(this.baseUrl + 'groups', group);
}

updateGroup(id: number, group: Group): Observable<void> {
  return this.http.put<void>(this.baseUrl + 'groups/' + id, group);
}

getInnerTeams(groupId: number): Observable<InnerTeams> {
  return this.http.get<InnerTeams>(this.baseUrl + 'groups/inner-teams/' + groupId);
}

getModeratorGroups(moderatorId: number): Observable<Group[]> {
  return this.http.get<Group[]>(this.baseUrl + 'groups/moderator-groups/' + moderatorId);
}

getModeratorFreeGroups(organizationId: number, moderator: ModeratorMin): Observable<Group[]> {
  return this.http.get<Group[]>(this.baseUrl + 'groups/moderator-free-groups/' + organizationId + '/moderator/' + moderator.id);
}

updateModeratorGroups(id: number, groups): Observable<void> {
  return this.http.put<void>(this.baseUrl + 'moderators/update-groups/' + id, groups);
}

changeStatus(id: number): Observable<void> {
  return this.http.put<void>(this.baseUrl + 'groups/change-status/', id);
}

}
