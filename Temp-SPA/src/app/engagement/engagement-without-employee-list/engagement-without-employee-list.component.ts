import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Employee } from 'src/app/models/employee';
import { PaginatedResult, Pagination } from 'src/app/models/pagination';
import { AlertifyService } from 'src/app/services/alertify.service';
import { EngagementService } from 'src/app/services/engagement.service';

@Component({
  selector: 'app-engagement-without-employee-list',
  templateUrl: './engagement-without-employee-list.component.html'
})
export class EngagementWithoutEmployeeListComponent implements OnInit {
  employees: Employee[];
  pagination: Pagination;

  constructor(
    private route: ActivatedRoute,
    private engagementService: EngagementService,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.employees = data['employeesWithout'].result;
      this.pagination = data['employeesWithout'].pagination;
    });
  }

  loadEmployeesWithoutEngagement(): void {
    this.engagementService.getEmpoyeesWithoutEngagement(this.pagination.currentPage, this.pagination.itemsPerPage)
      .subscribe((res: PaginatedResult<Employee[]>) => {
        this.employees = res.result;
        this.pagination = res.pagination;
      }, error => {
        this.alertify.error(error.error);
      })
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadEmployeesWithoutEngagement();
  }

}
