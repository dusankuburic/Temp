import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { EmploymentStatus } from '../_models/employmentStatus';
import { PaginatedResult } from '../_models/pagination';

@Injectable({
  providedIn: 'root'
})
export class EmploymentStatusService {
  baseUrl = environment.apiUrl;

constructor(private http: HttpClient) { }

getEmploymentStatuses(page?, itemsPerPage?): Observable<PaginatedResult<EmploymentStatus[]>> {
  const paginatedResult: PaginatedResult<EmploymentStatus[]> = new PaginatedResult<EmploymentStatus[]>();

  let params = new HttpParams();

  if(page != null && itemsPerPage != null){
    params = params.append('pageNumber', page);
    params = params.append('pageSize', itemsPerPage);
  }

  return this.http.get<EmploymentStatus[]>(this.baseUrl + 'employmentStatuses', {observe: 'response', params})
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

getEmploymentStatus(id: number): any {
  return this.http.get<EmploymentStatus>(this.baseUrl + 'employmentStatuses/' + id);
}

createEmploymentStatus(employmentStatus: EmploymentStatus): any {
  return this.http.post(this.baseUrl + 'employmentStatuses', employmentStatus);
}

updateEmploymentStatus(id: any, employmentStatus: EmploymentStatus): any {
  return this.http.put(this.baseUrl + 'employmentStatuses/' + id, employmentStatus);
}

}
