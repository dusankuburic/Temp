import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faEdit, faLock, faLockOpen, faPlus, faPlusCircle, faPlusSquare, faSitemap, faUserTimes } from '@fortawesome/free-solid-svg-icons';
import { debounce, debounceTime, last } from 'rxjs';
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
  plusIcon = faPlusCircle;

  filtersForm: FormGroup;
  employees: Employee[];
  unassignRoleDto: UnassignRoleDto;
  roles = [
    {value: '', display: 'Select Role', disabled: true},
    {value: 'User', display: 'User',  disabled: false},
    {value: 'Admin', display: 'Admin', disabled: false},
    {value: 'Moderator', display: 'Moderator', disabled: false},
    {value: 'None', display: 'None', disabled: false}];
  employeeParams: any = { role: '', firstName: '', lastName: '' };
  pagination: Pagination;

  constructor(
    private route: ActivatedRoute,
    private employeeService: EmployeeService,
    private alertify: AlertifyService,
    private fb: FormBuilder) { 
      this.filtersForm = this.fb.group({
        role: ['', Validators.minLength(2)],
        firstName: ['', Validators.minLength(1)],
        lastName: ['', Validators.minLength(1)]
      });

      const roleControl = this.filtersForm.get('role');
      roleControl.valueChanges.pipe(
        debounceTime(100)
      ).subscribe(() => {
        if (roleControl.value !== null) {
          this.employeeParams.role = roleControl.value;
          this.loadEmployees();
        }
      });
      
      const firstNameControl = this.filtersForm.get('firstName');
      firstNameControl.valueChanges.pipe(
        debounceTime(1000)
      ).subscribe(() => {
        if (firstNameControl.value !== null) {
          this.employeeParams.firstName = firstNameControl.value;
          this.loadEmployees();
        }
      });

      const lastNameControl = this.filtersForm.get('lastName');
      lastNameControl.valueChanges.pipe(
        debounceTime(1000)
      ).subscribe(() => {
        if (lastNameControl.value !== null) {
          this.employeeParams.lastName = lastNameControl.value;
          this.loadEmployees();
        }
      });
    }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.employees = data['employees'].result;
      this.pagination = data['employees'].pagination;
    });
  }

  loadEmployees(): void {
    this.employeeService.getEmployees(this.pagination.currentPage, this.pagination.itemsPerPage, this.employeeParams)
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

  resetFilters(): void {
    this.employeeParams.role = '';
    this.employeeParams.firstName = '';
    this.employeeParams.lastName = '';
    this.filtersForm.patchValue({
      role: '',
      firstName: '',
      lastName: ''
    });
    this.loadEmployees();
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadEmployees();
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
