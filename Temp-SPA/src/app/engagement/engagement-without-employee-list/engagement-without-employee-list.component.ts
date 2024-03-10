import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faCodeBranch } from '@fortawesome/free-solid-svg-icons';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { Employee } from 'src/app/core/models/employee';
import { EngagementParams } from 'src/app/core/models/engagement';
import { PaginatedResult, Pagination } from 'src/app/core/models/pagination';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EngagementService } from 'src/app/core/services/engagement.service';
import { SelectionOption } from 'src/app/shared/components/tmp-select/tmp-select.component';

@Component({
  selector: 'app-engagement-without-employee-list',
  templateUrl: './engagement-without-employee-list.component.html'
})
export class EngagementWithoutEmployeeListComponent implements OnInit {
  addEngagementIcon = faCodeBranch

  filtersForm: FormGroup;
  employees: Employee[];
  rolesSelect: SelectionOption[] = [
    {value: '', display: 'Select Role', disabled: true},
    {value: '', display: 'All'},
    {value: 'User', display: 'User'},
    {value: 'Admin', display: 'Admin'},
    {value: 'Moderator', display: 'Moderator'},
    {value: 'None', display: 'None'}];
  engagementParams: EngagementParams;
  pagination: Pagination;
  
  constructor(
    private route: ActivatedRoute,
    private engagementService: EngagementService,
    private alertify: AlertifyService,
    private fb: FormBuilder) { 
      this.engagementParams = engagementService.getEngagementParams();

      this.filtersForm = this.fb.group({
        role: [''],
        firstName: [''],
        lastName: ['']
      });

      const roleControl = this.filtersForm.get('role');
      roleControl.valueChanges.pipe(
        debounceTime(100),
        distinctUntilChanged(),
      ).subscribe((searchFor) => {
        const params = this.engagementService.getEngagementParams();
        params.pageNumber = 1;
        this.engagementParams.role = searchFor;
        this.engagementService.setEngagementParams(params);
        this.engagementParams = params;
        this.loadEmployeesWithoutEngagement();
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
        this.loadEmployeesWithoutEngagement();
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
        this.loadEmployeesWithoutEngagement();
      });
    }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.employees = data['employeesWithout'].result;
      this.pagination = data['employeesWithout'].pagination;
    });
  }

  loadEmployeesWithoutEngagement(): void {
    this.engagementService.getEmployeesWithoutEngagement()
    .subscribe({
      next: (res: PaginatedResult<Employee[]>) => {
        this.employees = res.result;
        this.pagination = res.pagination;
      },
      error: () => {
        this.alertify.error('Unable to list employees');
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
      this.loadEmployeesWithoutEngagement();
    }
  }

}
