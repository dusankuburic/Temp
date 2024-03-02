import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faCodeBranch } from '@fortawesome/free-solid-svg-icons';
import { debounceTime, distinctUntilChanged, last } from 'rxjs';
import { Employee } from 'src/app/core/models/employee';
import { EngagementParams } from 'src/app/core/models/engagement';
import { PaginatedResult, Pagination } from 'src/app/core/models/pagination';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EngagementService } from 'src/app/core/services/engagement.service';

@Component({
  selector: 'app-engagement-with-employee-list',
  templateUrl: './engagement-with-employee-list.component.html'
})
export class EngagementWithEmployeeListComponent implements OnInit {
  addEngagementIcon = faCodeBranch

  filtersForm: FormGroup;
  roles = [
    {value: '', display: 'Select Role', disabled: true},
    {value: '', display: 'All', disabled: false},
    {value: 'User', display: 'User',  disabled: false},
    {value: 'Admin', display: 'Admin', disabled: false},
    {value: 'Moderator', display: 'Moderator', disabled: false},
    {value: 'None', display: 'None', disabled: false}];
  employees: Employee[];
  pagination: Pagination;
  engagementParams: EngagementParams;

  constructor(
    private route: ActivatedRoute,
    private engagementService: EngagementService,
    private alertify: AlertifyService,
    private fb: FormBuilder) { 
      this.engagementParams = engagementService.getEngagementParams();

      this.filtersForm = this.fb.group({
        role: ['', Validators.minLength(1)],
        firstName: ['', Validators.minLength(1)],
        lastName: ['', Validators.minLength(1)],
        workplace: ['', Validators.minLength(1)],
        employmentStatus: ['', Validators.minLength(1)],
        minSalary: [0, [Validators.min(0), Validators.max(5000)]],
        maxSalary: [5000, [Validators.min(0), Validators.max(5000)]]
      });

      const roleControl = this.filtersForm.get('role');
      roleControl.valueChanges.pipe(
        debounceTime(100),
        distinctUntilChanged()
      ).subscribe((searchFor) => {
        const params = this.engagementService.getEngagementParams();
        params.pageNumber = 1;
        this.engagementParams.role = searchFor;
        this.engagementService.setEngagementParams(params);
        this.engagementParams = params;
        this.loadEmployeesWithEngagement();
      });

      const firstNameControl = this.filtersForm.get('firstName');
      firstNameControl.valueChanges.pipe(
        debounceTime(600),
        distinctUntilChanged()
      ).subscribe((searchFor) => {
        const params = this.engagementService.getEngagementParams();
        params.pageNumber = 1;
        params.firstName = searchFor;
        this.engagementService.setEngagementParams(params);
        this.engagementParams = params;
        this.loadEmployeesWithEngagement();
      });

      const lastNameControl = this.filtersForm.get('lastName');
      lastNameControl.valueChanges.pipe(
        debounceTime(600),
        distinctUntilChanged()
      ).subscribe((searchFor) => {
        const params = this.engagementService.getEngagementParams();
        params.pageNumber = 1;
        params.lastName = searchFor;
        this.engagementService.setEngagementParams(params);
        this.engagementParams = params;
        this.loadEmployeesWithEngagement();
      });

      const workplaceControl = this.filtersForm.get('workplace');
      workplaceControl.valueChanges.pipe(
        debounceTime(600),
        distinctUntilChanged()
      ).subscribe((searchFor) => {
        const params = this.engagementService.getEngagementParams();
        params.pageNumber = 1;
        params.workplace = searchFor;
        this.engagementService.setEngagementParams(params);
        this.engagementParams = params;
        this.loadEmployeesWithEngagement();
      });

      const employmentStatusControl = this.filtersForm.get('employmentStatus');
      employmentStatusControl.valueChanges.pipe(
        debounceTime(600),
        distinctUntilChanged()
      ).subscribe((searchFor) => {
        const params = this.engagementService.getEngagementParams();
        params.pageNumber = 1;
        params.employmentStatus = searchFor;
        this.engagementService.setEngagementParams(params);
        this.engagementParams = params;
        this.loadEmployeesWithEngagement();
      });

      const minSalaryControl = this.filtersForm.get('minSalary');
      minSalaryControl.valueChanges.pipe(
        debounceTime(600),
        distinctUntilChanged()
      ).subscribe((searchFor) => {
        const params = this.engagementService.getEngagementParams();
        params.pageNumber = 1;
        params.minSalary = searchFor;
        this.engagementService.setEngagementParams(params);
        this.engagementParams = params;
        this.loadEmployeesWithEngagement();
      });


      const maxSalaryControl = this.filtersForm.get('maxSalary');
      maxSalaryControl.valueChanges.pipe(
        debounceTime(600),
        distinctUntilChanged()
      ).subscribe((searchFor) => {
        const params = this.engagementService.getEngagementParams();
        params.pageNumber = 1;
        params.maxSalary = searchFor;
        this.engagementService.setEngagementParams(params);
        this.engagementParams = params;
        this.loadEmployeesWithEngagement();
      });

    }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.employees = data['employeesWith'].result;
      this.pagination = data['employeesWith'].pagination;
    });
  }

  loadEmployeesWithEngagement(): void {
    this.engagementService.getEmployeesWithEngagement().subscribe({
        next: (res: PaginatedResult<Employee[]>) => {
          this.employees = res.result;
          this.pagination = res.pagination;
        },
        error: (error) => {
          this.alertify.error(error.error);
        }
      });
  }

  pageChanged(event: any): void {
    const params = this.engagementService.getEngagementParams();
    if (params.pageNumber !== event) {
      this.pagination.currentPage = event;
      params.pageNumber = event;
      this.engagementService.setEngagementParams(params);
      this.engagementParams = params;
      this.loadEmployeesWithEngagement();
    }
  }

}
