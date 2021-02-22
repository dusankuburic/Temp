import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Employee } from '../_models/employee';

@Injectable({
  providedIn: 'root'
})
export class EngagementService {
  baseUrl = environment.apiUrl;

constructor(private http: HttpClient) { }

getEmpoyeesWithEngagement(): any {
  return this.http.get<Employee[]>(this.baseUrl + 'engagements/with');
}

getEmpoyeesWithoutEngagement(): any {
  return this.http.get<Employee[]>(this.baseUrl + 'engagements/without');
}

getEngagementForEmployee(id: number): any {
  return this.http.get(this.baseUrl + 'engagements/employee/' + id);
}

}
