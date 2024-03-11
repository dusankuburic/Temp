import { AfterViewInit, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faEdit, faLock, faLockOpen, faPlusCircle, faSitemap, faUserTimes } from '@fortawesome/free-solid-svg-icons';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { Employee, EmployeeParams } from 'src/app/core/models/employee';
import { PaginatedResult, Pagination } from 'src/app/core/models/pagination';
import { UnassignRoleDto } from 'src/app/core/models/unassignRoleDto';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EmployeeService } from 'src/app/core/services/employee.service';
import { SelectionOption } from 'src/app/shared/components/tmp-select/tmp-select.component';

@Component({
  selector: 'app-employee-list',
  templateUrl: './employee-list.component.html'
})
export class EmployeeListComponent implements OnInit, AfterViewInit {
  editIcon = faEdit
  assignRoleIcon = faSitemap
  removeRoleIcon = faUserTimes
  activateUserIcon = faLock
  deactivateUserIcon = faLockOpen
  plusIcon = faPlusCircle;

  filtersForm: FormGroup;
  employees: Employee[];
  unassignRoleDto: UnassignRoleDto;
  rolesSelect: SelectionOption[] = [
    {value: '', display: 'Select Role', disabled: true},
    {value: '', display: 'All'},
    {value: 'User', display: 'User'},
    {value: 'Admin', display: 'Admin'},
    {value: 'Moderator', display: 'Moderator'},
    {value: 'None', display: 'None'}];
  employeeParams: EmployeeParams;
  pagination: Pagination;

  constructor(
    private route: ActivatedRoute,
    private employeeService: EmployeeService,
    private alertify: AlertifyService,
    private fb: FormBuilder) {  
      this.employeeParams = employeeService.getEmployeeParams();

      this.filtersForm = this.fb.group({
        role: [''],
        firstName: [null],
        lastName: [null]
      });
    }
    
  ngAfterViewInit(): void {
    const roleControl = this.filtersForm.get('role');
    roleControl.valueChanges.pipe(
      debounceTime(100),
      distinctUntilChanged()
    ).subscribe((searchFor) => {
        const params = this.employeeService.getEmployeeParams();
        params.pageNumber = 1;
        this.employeeParams.role = searchFor;
        this.employeeService.setEmployeeParams(params);
        this.employeeParams = params;
        this.loadEmployees();
    });
    
    const firstNameControl = this.filtersForm.get('firstName');
    firstNameControl.valueChanges.pipe(
      debounceTime(600),
      distinctUntilChanged(),
    ).subscribe((searchFor) => {
        const params = this.employeeService.getEmployeeParams();
        params.pageNumber = 1;
        params.firstName = searchFor;
        this.employeeService.setEmployeeParams(params);
        this.employeeParams = params;
        this.loadEmployees();
    });

    const lastNameControl = this.filtersForm.get('lastName');
    lastNameControl.valueChanges.pipe(
      debounceTime(600),
      distinctUntilChanged(),
    ).subscribe((searchFor) => {
      const params = this.employeeService.getEmployeeParams();
      params.pageNumber = 1;
      params.lastName = searchFor;
      this.employeeService.setEmployeeParams(params);
      this.employeeParams = params;
      this.loadEmployees();
    });
  }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.employees = data['employees'].result;
      this.pagination = data['employees'].pagination;
    });
  }

  loadEmployees(): void {
    this.employeeService.getEmployees()
      .subscribe({
        next: (res: PaginatedResult<Employee[]>) => {
          this.employees = res.result;
          this.pagination = res.pagination;
        },
        error: () => {
          this.alertify.error('Unable to load employees');
        }
      });
  }

  pageChanged(event: any): void {
    const params = this.employeeService.getEmployeeParams();
    if (params.pageNumber !== event) {
      this.pagination.currentPage = event;
      params.pageNumber = event;
      this.employeeService.setEmployeeParams(params);
      this.employeeParams = params;
      this.loadEmployees();
    }
  }

  removeRole(id: number): void {
    this.unassignRoleDto = {id: id};
    this.employeeService.unassignRole(this.unassignRoleDto).subscribe({
      next: () => {
        this.loadEmployees();
        this.alertify.success('Remove role');
      },
      error: () => {
        this.alertify.error('Unable to remove role');
      }
    });
  }

  changeStatus(id: number): void {
    this.unassignRoleDto = {id: id};
    this.employeeService.changeStatus(id).subscribe({
      next: () => {
        this.loadEmployees();
        this.alertify.success('Status changed');
      },
      error: () => {
        this.alertify.error('Unable to change status');
      }
    });
  }


}
