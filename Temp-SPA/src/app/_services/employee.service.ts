import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { AssignRoleDto } from '../_models/assignRoleDto';
import { Employee } from '../_models/employee';
import { Moderator } from '../_models/moderator';
import { PaginatedResult } from '../_models/pagination';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  baseUrl = environment.apiUrl;

constructor(private http: HttpClient) { }


getEmployees(page?, itemsPerPage?, employeeParams?): Observable<PaginatedResult<Employee[]>> {
  const paginatedResult: PaginatedResult<Employee[]> = new PaginatedResult<Employee[]>();

  let params = new HttpParams();

  if (page != null && itemsPerPage != null)
  {
    params = params.append('pageNumber', page);
    params = params.append('pageSize', itemsPerPage);
  }

  if (employeeParams != null)
  {
    params = params.append('role', employeeParams.role);
    params = params.append('firstName', employeeParams.firstName);
    params = params.append('lastName', employeeParams.lastName);
  }

  return this.http.get<Employee[]>(this.baseUrl + 'employees', {observe : 'response', params})
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

getModerator(EmployeeId: number): any {
  return this.http.get<Moderator>(this.baseUrl + 'moderators/' + EmployeeId);
}

getEmployee(id: number): any {
  return this.http.get<Employee>(this.baseUrl + 'employees/' + id);
}

createEmployee(employee: Employee): any {
  return this.http.post(this.baseUrl + 'employees', employee);
}

updateEmployee(id: any, employee: Employee): any {
  return this.http.put(this.baseUrl + 'employees/' + id, employee);
}

assignRole(entity: AssignRoleDto): any {
  return this.http.post(this.baseUrl + 'employees/assign', entity);
}

unassignRole(entity: any): any {
  return this.http.post(this.baseUrl + 'employees/unassign', entity);
}

}
