import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Employee } from 'src/app/models/employee';
import { PaginatedResult, Pagination } from 'src/app/models/pagination';
import { AlertifyService } from 'src/app/services/alertify.service';
import { EngagementService } from 'src/app/services/engagement.service';

@Component({
  selector: 'app-engagement-with-employee-list',
  templateUrl: './engagement-with-employee-list.component.html'
})
export class EngagementWithEmployeeListComponent implements OnInit {
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
    this.engagementService.getEmpoyeesWithEngagement(
      this.pagination.currentPage,
      this.pagination.itemsPerPage,
      this.employeeParams)
    .subscribe((res: PaginatedResult<Employee[]>) => {
      this.employees = res.result;
      this.pagination = res.pagination;
    }, error => {
      this.alertify.error(error.error);
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
