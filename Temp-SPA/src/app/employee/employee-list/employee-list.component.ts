import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faEdit, faLock, faLockOpen, faSitemap, faUserTimes } from '@fortawesome/free-solid-svg-icons';
import { Employee } from 'src/app/core/models/employee';
import { PaginatedResult, Pagination } from 'src/app/core/models/pagination';
import { UnassignRoleDto } from 'src/app/core/models/unassignRoleDto';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EmployeeService } from 'src/app/core/services/employee.service';

@Component({
  selector: 'app-employee-list',
  templateUrl: './employee-list.component.html'
})
export class EmployeeListComponent implements OnInit {
  editIcon = faEdit
  assignRoleIcon = faSitemap
  removeRoleIcon = faUserTimes
  activateUserIcon = faLock
  deactivateUserIcon = faLockOpen

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

  removeRole(id: number): void {
    this.unassignRoleDto.id = id;
    this.employeeService.unassignRole(this.unassignRoleDto).subscribe({
      next: () => {
        this.loadEmployees();
        this.alertify.success('Remove role');
      },
      error: (error) => {
        this.alertify.error(error.error);
      }
    });
  }

  changeStatus(id: number): void {
    this.unassignRoleDto.id = id;
    this.employeeService.changeStatus(id).subscribe({
      next: () => {
        this.loadEmployees();
        this.alertify.success('Status changed');
      },
      error: (error) => {
        this.alertify.error(error.error);
      }
    });
  }


}
