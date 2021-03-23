import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { CreateApplication, ModeratorListApplication, Application, UserListApplication } from '../_models/application';

@Injectable({
  providedIn: 'root'
})
export class ApplicationService {
  baseUrl = environment.apiUrl;

constructor(private http: HttpClient) { }


getApplication(id: number): any {
  return this.http.get<Application>(this.baseUrl + 'applications/' + id);
}

createApplication(application: CreateApplication): any {
  return this.http.post(this.baseUrl + 'applications', application);
}

getTeamApplicationsForModerator(teamId: number): any {
  return this.http.get<ModeratorListApplication[]>(this.baseUrl + 'applications/team/' + teamId);
}

getUserApplications(userId: number): any {
  return this.http.get<UserListApplication[]>(this.baseUrl + 'applications/user/' + userId);
}

}
