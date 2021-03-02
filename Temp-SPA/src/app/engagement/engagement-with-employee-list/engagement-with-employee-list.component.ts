import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Employee } from 'src/app/_models/employee';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { EngagementService } from 'src/app/_services/engagement.service';

@Component({
  selector: 'app-engagement-with-employee-list',
  templateUrl: './engagement-with-employee-list.component.html',
  styleUrls: ['./engagement-with-employee-list.component.scss']
})
export class EngagementWithEmployeeListComponent implements OnInit {
  employees: Employee[];
  pagination: Pagination;

  constructor(
    private route: ActivatedRoute,
    private engagementService: EngagementService,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.employees = data['employeesWith'].result;
      this.pagination = data['employeesWith'].pagination;
    });
  }

  loadEmployeesWithEngagement(): void {
    this.engagementService.getEmpoyeesWithEngagement(this.pagination.currentPage, this.pagination.itemsPerPage)
      .subscribe((res: PaginatedResult<Employee[]>) => {
        this.employees = res.result;
        this.pagination = res.pagination;
      }, error => {
        this.alertify.error(error.error);
      })
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadEmployeesWithEngagement();
  }

}
