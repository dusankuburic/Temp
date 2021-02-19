import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { AssignRoleDto } from '../_models/assignRoleDto';
import { Employee } from '../_models/employee';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  baseUrl = environment.apiUrl;

constructor(private http: HttpClient) { }


getEmployees(): any {
  return this.http.get<Employee[]>(this.baseUrl + 'employees');
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
