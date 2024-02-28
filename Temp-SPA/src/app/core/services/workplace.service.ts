import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { PaginatedResult } from '../models/pagination';
import { UpdateWorkplaceStatus, Workplace } from '../models/workplace';

@Injectable({
  providedIn: 'root'
})
export class WorkplaceService {
  baseUrl = environment.apiUrl;

constructor(private http: HttpClient) { }


getPagedWorkplaces(page?, itemsPerPage?): Observable<PaginatedResult<Workplace[]>> {
  const paginatedResult: PaginatedResult<Workplace[]> = new PaginatedResult<Workplace[]>();

  let params = new HttpParams();

  if (page != null && itemsPerPage != null) {
    params = params.append('pageNumber', page);
    params = params.append('pageSize', itemsPerPage);
  }
  

  return this.http.get<Workplace[]>(this.baseUrl + 'workplaces/paged-workplaces', {observe: 'response', params})
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

getWorkplaces(): Observable<Workplace[]> {
  return this.http.get<Workplace[]>(this.baseUrl + 'workplaces');
}

getWorkplace(id: number): Observable<Workplace> {
  return this.http.get<Workplace>(this.baseUrl + 'workplaces/' + id);
}

createWorkplace(workplace: Workplace): Observable<Workplace> {
  return this.http.post<Workplace>(this.baseUrl + 'workplaces', workplace);
}

updateWorkplace(workplace: Workplace): Observable<void> {
  return this.http.put<void>(this.baseUrl + 'workplaces/' + workplace.id, workplace);
}

changeStatus(request: UpdateWorkplaceStatus): Observable<void> {
  return this.http.put<void>(this.baseUrl + 'workplaces/change-status/' + request.id, request);  
}


}


