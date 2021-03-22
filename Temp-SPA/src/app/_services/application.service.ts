import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { CreateApplication, ModeratorListApplication } from '../_models/application';

@Injectable({
  providedIn: 'root'
})
export class ApplicationService {
  baseUrl = environment.apiUrl;

constructor(private http: HttpClient) { }


createApplication(application: CreateApplication): any {
  return this.http.post(this.baseUrl + 'applications', application);
}

getTeamApplications(teamId: number): any {
  return this.http.get<ModeratorListApplication[]>(this.baseUrl + 'applications/team/' + teamId);
}


}
