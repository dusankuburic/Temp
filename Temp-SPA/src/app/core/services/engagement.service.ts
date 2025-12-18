import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Employee } from '../models/employee';
import { Engagement, EngagementParams } from '../models/engagement';
import { PaginatedResult } from '../models/pagination';

@Injectable({
  providedIn: 'root'
})
export class EngagementService {
  baseUrl = environment.apiUrl;
  engagementParams = new EngagementParams();

constructor(private http: HttpClient) { }

setEngagementParams(params: EngagementParams): void {
  this.engagementParams = params;
}

getEngagementParams(): EngagementParams {
  return this.engagementParams;
}

resetEngagementParams(): void {
  this.engagementParams.pageNumber = 1;
  this.engagementParams.pageSize = 10;
  this.engagementParams.role = '';
  this.engagementParams.firstName = '';
  this.engagementParams.lastName = '';
}

getEmployeesWithEngagement(): Observable<PaginatedResult<Employee[]>> {
  const paginatedResult: PaginatedResult<Employee[]> = new PaginatedResult<Employee[]>();

  let params = new HttpParams();

  params = params.append('pageNumber', this.engagementParams.pageNumber);
  params = params.append('pageSize', this.engagementParams.pageSize);


  if (this.engagementParams.role) {
    params = params.append('role', this.engagementParams.role);
  }
  if (this.engagementParams.firstName) {
    params = params.append('firstName', this.engagementParams.firstName);
  }
  if (this.engagementParams.lastName) {
    params = params.append('lastName', this.engagementParams.lastName);
  }

  return this.http.get<Employee[]>(this.baseUrl + 'engagements/with', {observe: 'response', params})
    .pipe(
      map(response => {
        paginatedResult.result = response.body ?? [];
        if (response.headers.get('Pagination') != null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination') ?? '{}');
        }
        return paginatedResult;
      })
    );
}

getEmployeesWithoutEngagement(): Observable<PaginatedResult<Employee[]>> {
  const paginatedResult: PaginatedResult<Employee[]> = new PaginatedResult<Employee[]>();

  let params = new HttpParams();

  params = params.append('pageNumber', this.engagementParams.pageNumber);
  params = params.append('pageSize', this.engagementParams.pageSize);

  if (this.engagementParams.role) {
    params = params.append('role', this.engagementParams.role);
  }
  if (this.engagementParams.firstName) {
    params = params.append('firstName', this.engagementParams.firstName);
  }
  if (this.engagementParams.lastName) {
    params = params.append('lastName', this.engagementParams.lastName);
  }

  return this.http.get<Employee[]>(this.baseUrl + 'engagements/without', {observe: 'response', params})
    .pipe(
      map(response => {
        paginatedResult.result = response.body ?? [];
        if (response.headers.get('Pagination') != null){
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination') ?? '{}');
        }
        return paginatedResult;
      })
    );
}

getEngagementForEmployee(id: number): any {
  return this.http.get(this.baseUrl + 'engagements/employee/' + id);
}

getUserEmployeeEngagements(id: number): any {
  return this.http.get(this.baseUrl + 'engagements/user/' + id);
}

createEngagement(engagement: Engagement): Observable<Engagement> {
  return this.http.post<Engagement>(this.baseUrl + 'engagements', engagement);
}

}
