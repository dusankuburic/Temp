import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Employee } from '../models/employee';
import { Engagement } from '../models/engagement';
import { PaginatedResult } from '../models/pagination';

@Injectable({
  providedIn: 'root'
})
export class EngagementService {
  baseUrl = environment.apiUrl;

constructor(private http: HttpClient) { }

getEmployeesWithEngagement(page?, itemsPerPage?, employeeParams?): Observable<PaginatedResult<Employee[]>> {
  const paginatedResult: PaginatedResult<Employee[]> = new PaginatedResult<Employee[]>();

  let params = new HttpParams();

  if (page != null && itemsPerPage != null){
    params = params.append('pageNumber', page);
    params = params.append('pageSize', itemsPerPage);
  }


  if (employeeParams != null)
  {
    params = params.append('minSalary', employeeParams.minSalary);
    params = params.append('maxSalary', employeeParams.maxSalary);

    if (employeeParams.workplace !== '' && employeeParams.employmentStatus !== '') {
      params = params.append('workplace', employeeParams.workplace);
      params = params.append('employmentStatus', employeeParams.employmentStatus);
    } else if (employeeParams.workplace !== '') {
      params = params.append('workplace', employeeParams.workplace);
    } else if (employeeParams.employmentStatus !== '') {
      params = params.append('employmentStatus', employeeParams.employmentStatus);
    }
  }

  return this.http.get<Employee[]>(this.baseUrl + 'engagements/with', {observe: 'response', params})
    .pipe(
      map(response => {
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') != null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      })
    );
}

getEmployeesWithoutEngagement(page?, itemsPerPage?): Observable<PaginatedResult<Employee[]>> {
  const paginatedResult: PaginatedResult<Employee[]> = new PaginatedResult<Employee[]>();

  let params = new HttpParams();

  if (page != null && itemsPerPage != null) {
    params = params.append('pageNumber', page);
    params = params.append('pageSize', itemsPerPage);
  }

  return this.http.get<Employee[]>(this.baseUrl + 'engagements/without', {observe: 'response', params})
    .pipe(
      map(response => {
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') != null){
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
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
