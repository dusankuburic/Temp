import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { CreateApplication, ModeratorListApplication, Application, UserListApplication, UpdateApplicationRequest } from '../models/application';

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

getTeamApplicationsForModerator(teamId: number, moderatorId: number): any {
  return this.http.get<ModeratorListApplication[]>(this.baseUrl + 'applications/team/' + teamId + "/moderator/" + moderatorId);
}

getUserApplications(userId: number): any {
  return this.http.get<UserListApplication[]>(this.baseUrl + 'applications/user/' + userId);
}

updateApplicationStatus(request: UpdateApplicationRequest): any {
  return this.http.put(this.baseUrl + 'applications/change-status/', request);
}

}
