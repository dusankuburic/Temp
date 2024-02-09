import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { FullTeam, Team } from '../models/team';

@Injectable({
  providedIn: 'root'
})
export class TeamService {
  baseUrl = environment.apiUrl;

constructor(private http: HttpClient) { }

getTeam(id: number): any {
  return this.http.get<Team>(this.baseUrl + 'teams/' + id);
}

getFullTeam(id: number): any {
  return this.http.get<FullTeam>(this.baseUrl + 'teams/full/' + id);
}

getTeams(groupId: number): any {
  return this.http.get<Team[]>(this.baseUrl + 'groups/inner-teams/' + groupId);
}

createTeam(team: Team): any {
  return this.http.post(this.baseUrl + 'teams', team);
}

updateTeam(id: number, team: Team): any {
  return this.http.put(this.baseUrl + 'teams/' + id, team);
}

getUserTeam(userId: number): any {
  return this.http.get(this.baseUrl + 'teams/employee/team/' + userId);
}

changeStatus(id: number): any {
  return this.http.put(this.baseUrl + 'teams/change-status/' + id, id);
}

}