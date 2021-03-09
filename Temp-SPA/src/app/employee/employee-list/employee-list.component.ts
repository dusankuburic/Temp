import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Employee } from 'src/app/_models/employee';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { UnassignRoleDto } from 'src/app/_models/unassignRoleDto';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { EmployeeService } from 'src/app/_services/employee.service';

@Component({
  selector: 'app-employee-list',
  templateUrl: './employee-list.component.html',
  styleUrls: ['./employee-list.component.scss']
})
export class EmployeeListComponent implements OnInit {
  employees: Employee[];
  unassignRoleDto = {} as UnassignRoleDto;
  roles = [
    {value: 'User', display: 'User'},
    {value: 'Admin', display: 'Admin'},
    {value: 'Moderator', display: 'Moderator'},
    {value: 'None', display: 'None'}];
  employeeParams: any = {};
  pagination: Pagination;

  constructor(
    private route: ActivatedRoute,
    private employeeService: EmployeeService,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.employees = data['employees'].result;
      this.pagination = data['employees'].pagination;
    });

    this.employeeParams.role = '';
    this.employeeParams.firstName = '';
    this.employeeParams.lastName = '';
  }

  loadEmployees(): void {
    this.employeeService.getEmployees(
      this.pagination.currentPage,
      this.pagination.itemsPerPage,
      this.employeeParams)
      .toPromise().then((res: PaginatedResult<Employee[]>) => {
      this.employees = res.result;
      this.pagination = res.pagination;
    }, error => {
      this.alertify.error(error.error);
    });
  }

  resetFilters(): void {
    this.employeeParams.role = '';
    this.employeeParams.firstName = '';
    this.employeeParams.lastName = '';
    this.loadEmployees();
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadEmployees();
  }

  removeRole(id: number): any {
    this.unassignRoleDto.id = id;
    this.employeeService.unassignRole(this.unassignRoleDto).subscribe(() => {
      this.loadEmployees();
      this.alertify.success('Removed role');
    }, error => {
      this.alertify.error(error.error);
    });
  }


}
