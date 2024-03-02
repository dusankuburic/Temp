import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { FullTeam, InnerTeams, PagedInnerTeams, Team, TeamParams } from '../models/team';
import { Observable, map } from 'rxjs';
import { PaginatedResult } from '../models/pagination';

@Injectable({
  providedIn: 'root'
})
export class TeamService {
  baseUrl = environment.apiUrl;
  teamParams = new TeamParams();

constructor(private http: HttpClient) { }

setTeamParams(params: TeamParams) {
  this.teamParams = params;
}

getTeamParams(): TeamParams {
  return this.teamParams;
}

resetTeamParams(): void {
  this.teamParams.pageNumber = 1;
  this.teamParams.pageSize = 5;
  this.teamParams.name = '';
}

getInnerTeams(groupId: number): Observable<PagedInnerTeams> {
  const paginatedResult: PaginatedResult<Team[]> = new PaginatedResult<Team[]>();

  let params = new HttpParams();

  params = params.append('groupId', groupId);
  params = params.append('pageNumber', this.teamParams.pageNumber);
  params = params.append('pageSize', this.teamParams.pageSize);

  if (this.teamParams.name) {
    params = params.append('name', this.teamParams.name);
  }

  return this.http.get<InnerTeams>(this.baseUrl + 'groups/paged-inner-teams', {observe: 'response', params})
    .pipe(
      map(response => {
        paginatedResult.result = response.body.teams;
        if (response.headers.get('Pagination') != null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return {
          id: response.body.id,
          name: response.body.name,
          teams: paginatedResult
        }
      })
    )
}

getTeams(groupId: number): Observable<Team[]> {
  return this.http.get<Team[]>(this.baseUrl + 'groups/inner-teams/' + groupId);
}

checkTeamExists(name: string, groupId: number): Observable<boolean> {
  return this.http.get<boolean>(this.baseUrl + `teams/team-exists?name=${name}&groupId=${groupId}`);
}

getTeam(id: number): Observable<Team> {
  return this.http.get<Team>(this.baseUrl + 'teams/' + id);
}


getFullTeam(id: number): Observable<FullTeam> {
  return this.http.get<FullTeam>(this.baseUrl + 'teams/full/' + id);
}


createTeam(team: Team): Observable<Team> {
  return this.http.post<Team>(this.baseUrl + 'teams', team);
}

updateTeam(id: number, team: Team): Observable<void> {
  return this.http.put<void>(this.baseUrl + 'teams/' + id, team);
}

getUserTeam(userId: number): any {
  return this.http.get(this.baseUrl + 'teams/employee/team/' + userId);
}

changeStatus(id: number): Observable<void> {
  return this.http.put<void>(this.baseUrl + 'teams/change-status/' + id, id);
}

}
