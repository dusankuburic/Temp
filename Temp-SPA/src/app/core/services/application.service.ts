import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { CreateApplication, ModeratorListApplication, Application, UserListApplication, UpdateApplicationRequest } from '../models/application';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApplicationService {
  baseUrl = environment.apiUrl;

constructor(private http: HttpClient) { }

getApplication(id: number): Observable<Application> {
  return this.http.get<Application>(this.baseUrl + 'applications/' + id);
}

createApplication(application: CreateApplication): Observable<CreateApplication> {
  return this.http.post<CreateApplication>(this.baseUrl + 'applications', application);
}

getTeamApplicationsForModerator(teamId: number, moderatorId: number): Observable<ModeratorListApplication[]> {
  return this.http.get<ModeratorListApplication[]>(this.baseUrl + 'applications/team/' + teamId + "/moderator/" + moderatorId);
}

getUserApplications(userId: number): Observable<UserListApplication[]> {
  return this.http.get<UserListApplication[]>(this.baseUrl + 'applications/user/' + userId);
}

updateApplicationStatus(request: UpdateApplicationRequest): Observable<void> {
  return this.http.put<void>(this.baseUrl + 'applications/change-status/' + request.id, request);
}

}
