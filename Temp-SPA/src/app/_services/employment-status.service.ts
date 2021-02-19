import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { EmploymentStatus } from '../_models/employmentStatus';

@Injectable({
  providedIn: 'root'
})
export class EmploymentStatusService {
  baseUrl = environment.apiUrl;

constructor(private http: HttpClient) { }

getEmploymentStatuses(): any {
  return this.http.get<EmploymentStatus[]>(this.baseUrl + 'employmentStatuses');
}

getEmploymentStatus(id: number): any {
  return this.http.get<EmploymentStatus>(this.baseUrl + 'employmentStatuses/' + id);
}

createEmploymentStatus(employmentStatus: EmploymentStatus): any {
  return this.http.post(this.baseUrl + 'employmentStatuses', employmentStatus);
}

}
