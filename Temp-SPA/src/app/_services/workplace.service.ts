import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { PaginatedResult } from '../_models/pagination';
import { Workplace } from '../_models/workplace';

@Injectable({
  providedIn: 'root'
})
export class WorkplaceService {
  baseUrl = environment.apiUrl;

constructor(private http: HttpClient) { }


getWorkplaces(page?, itemsPerPage?): Observable<PaginatedResult<Workplace[]>> {
  const paginatedResult: PaginatedResult<Workplace[]> = new PaginatedResult<Workplace[]>();

  let params = new HttpParams();

  if (page != null && itemsPerPage != null)
  {
    params = params.append('pageNumber', page);
    params = params.append('pageSize', itemsPerPage);
  }
  

  return this.http.get<Workplace[]>(this.baseUrl + 'workplaces', {observe: 'response', params})
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

getWorkplace(id: number): any {
  return this.http.get<Workplace>(this.baseUrl + 'workplaces/' + id);
}

createWorkplace(workplace: Workplace): any {
  return this.http.post(this.baseUrl + 'workplaces', workplace);
}

updateWorkplace(id: any, workplace: Workplace): any {
  return this.http.put(this.baseUrl + 'workplaces/' + id, workplace);
}

changeStatus(id: number): any {
  return this.http.put(this.baseUrl + 'workplaces/change-status/' + id, id);  
}


}


