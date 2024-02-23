import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faEdit, faPenNib } from '@fortawesome/free-solid-svg-icons';
import { EmploymentStatus } from 'src/app/core/models/employmentStatus';
import { PaginatedResult, Pagination } from 'src/app/core/models/pagination';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EmploymentStatusService } from 'src/app/core/services/employment-status.service';

@Component({
  selector: 'app-employment-status-list',
  templateUrl: './employment-status-list.component.html'
})
export class EmploymentStatusListComponent implements OnInit {
  editIcon = faEdit
  archiveIcon = faPenNib

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
    this.employmentStatusService.getPagedEmploymentStatuses(this.pagination.currentPage, this.pagination.itemsPerPage)
      .subscribe({
        next: (res: PaginatedResult<EmploymentStatus[]>) => {
          this.employmentStatuses = res.result;
          this.pagination = res.pagination;
        },
        error: (error) => {
          this.alertify.error(error.error);
        }
      });
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadEmploymentStatuses();
  }

  changeStatus(id: number): any {
    this.employmentStatusService.changeStatus(id).subscribe(() => {
      this.loadEmploymentStatuses();
      this.alertify.success('Status changed');
    }, error => {
      this.alertify.error(error.error);
    });
  }

}
