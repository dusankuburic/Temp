import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Team } from '../_models/team';

@Injectable({
  providedIn: 'root'
})
export class TeamService {
  baseUrl = environment.apiUrl;

constructor(private http: HttpClient) { }

getTeam(id: number): any {
  return this.http.get<Team>(this.baseUrl + 'teams/' + id);
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

}
