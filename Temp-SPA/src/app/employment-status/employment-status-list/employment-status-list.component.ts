import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { EmploymentStatus } from 'src/app/_models/employmentStatus';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { EmploymentStatusService } from 'src/app/_services/employment-status.service';

@Component({
  selector: 'app-employment-status-list',
  templateUrl: './employment-status-list.component.html'
})
export class EmploymentStatusListComponent implements OnInit {
  employmentStatuses: EmploymentStatus[];
  pagination: Pagination;

  constructor(
    private route: ActivatedRoute,
    private employmentStatusService: EmploymentStatusService,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.employmentStatuses = data['employmentStatuses'].result;
      this.pagination = data['employmentStatuses'].pagination;
    });
  }

  loadEmploymentStatuses(): void {
    this.employmentStatusService.getEmploymentStatuses(this.pagination.currentPage, this.pagination.itemsPerPage)
      .subscribe((res: PaginatedResult<EmploymentStatus[]>) => {
        this.employmentStatuses = res.result;
        this.pagination = res.pagination;
      }, error => {
        this.alertify.error(error.error);
      })
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadEmploymentStatuses();
  }

}
