import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { EmploymentStatus } from '../models/employmentStatus';
import { PaginatedResult } from '../models/pagination';

@Injectable({
  providedIn: 'root'
})
export class EmploymentStatusService {
  baseUrl = environment.apiUrl;

constructor(private http: HttpClient) { }

getPagedEmploymentStatuses(page?, itemsPerPage?): Observable<PaginatedResult<EmploymentStatus[]>> {
  const paginatedResult: PaginatedResult<EmploymentStatus[]> = new PaginatedResult<EmploymentStatus[]>();

  let params = new HttpParams();

  if (page != null && itemsPerPage != null) {
    params = params.append('pageNumber', page);
    params = params.append('pageSize', itemsPerPage);
  }

  return this.http.get<EmploymentStatus[]>(this.baseUrl + 'employmentStatuses/paged-employmentstatuses', {observe: 'response', params})
    .pipe(
      map(response => {
        paginatedResult.result = response.body;
        if(response.headers.get('Pagination') != null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      })
    );
}

getEmploymentStatuses(): Observable<EmploymentStatus[]> {
  return this.http.get<EmploymentStatus[]>(this.baseUrl + 'employmentStatuses');
}

getEmploymentStatus(id: number): Observable<EmploymentStatus> {
  return this.http.get<EmploymentStatus>(this.baseUrl + 'employmentStatuses/' + id);
}

createEmploymentStatus(employmentStatus: EmploymentStatus): Observable<EmploymentStatus> {
  return this.http.post<EmploymentStatus>(this.baseUrl + 'employmentStatuses', employmentStatus);
}

updateEmploymentStatus(employmentStatus: EmploymentStatus): Observable<void> {
  return this.http.put<void>(this.baseUrl + 'employmentStatuses/' + employmentStatus.id, employmentStatus);
}

changeStatus(id: number): Observable<void> {
  return this.http.put<void>(this.baseUrl + 'employmentStatuses/change-status/' + id, {id});
}

}
