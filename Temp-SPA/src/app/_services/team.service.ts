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

getTeams(groupId: number): any {
  return this.http.get<Team[]>(this.baseUrl + 'groups/inner-teams/' + groupId);
}

}
