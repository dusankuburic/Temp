import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Employee } from '../_models/employee';
import { Engagement } from '../_models/engagement';
import { PaginatedResult } from '../_models/pagination';

@Injectable({
  providedIn: 'root'
})
export class EngagementService {
  baseUrl = environment.apiUrl;

constructor(private http: HttpClient) { }

getEmpoyeesWithEngagement(page?, itemsPerPage?): Observable<PaginatedResult<Employee[]>> {
  const paginatedResult: PaginatedResult<Employee[]> = new PaginatedResult<Employee[]>();

  let params = new HttpParams();

  if(page != null && itemsPerPage != null){
    params = params.append('pageNumber', page);
    params = params.append('pageSize', itemsPerPage);
  }

  return this.http.get<Employee[]>(this.baseUrl + 'engagements/with', {observe: 'response', params})
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

getEmpoyeesWithoutEngagement(): any {
  return this.http.get<Employee[]>(this.baseUrl + 'engagements/without');
}

getEngagementForEmployee(id: number): any {
  return this.http.get(this.baseUrl + 'engagements/employee/' + id);
}

createEngagement(engagement: Engagement): any {
  return this.http.post(this.baseUrl + 'engagements', engagement);
}

}
