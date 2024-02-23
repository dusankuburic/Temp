import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faCodeBranch } from '@fortawesome/free-solid-svg-icons';
import { Employee } from 'src/app/core/models/employee';
import { PaginatedResult, Pagination } from 'src/app/core/models/pagination';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EngagementService } from 'src/app/core/services/engagement.service';

@Component({
  selector: 'app-engagement-with-employee-list',
  templateUrl: './engagement-with-employee-list.component.html'
})
export class EngagementWithEmployeeListComponent implements OnInit {
  addEngagementIcon = faCodeBranch

  employees: Employee[];
  pagination: Pagination;
  employeeParams: any = {};

  constructor(
    private route: ActivatedRoute,
    private engagementService: EngagementService,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.employees = data['employeesWith'].result;
      this.pagination = data['employeesWith'].pagination;
    });

    this.employeeParams.workplace = '';
    this.employeeParams.employmentStatus = '';
    this.employeeParams.minSalary = 0;
    this.employeeParams.maxSalary = 5000;
  }

  loadEmployeesWithEngagement(): void {
    this.engagementService.getEmployeesWithEngagement(
      this.pagination.currentPage,
      this.pagination.itemsPerPage,
      this.employeeParams).subscribe({
        next: (res: PaginatedResult<Employee[]>) => {
          this.employees = res.result;
          this.pagination = res.pagination;
        },
        error: (error) => {
          this.alertify.error(error.error);
        }
      });
  }

  resetFilters(): void {
    this.employeeParams.workplace = '';
    this.employeeParams.employmentStatus = '';
    this.employeeParams.minSalary = 0;
    this.employeeParams.maxSalary = 5000;
    this.loadEmployeesWithEngagement();
  }


  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadEmployeesWithEngagement();
  }

}
