import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faEdit, faPlusCircle, faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { EmploymentStatus, EmploymentStatusParams } from 'src/app/core/models/employmentStatus';
import { PaginatedResult, Pagination } from 'src/app/core/models/pagination';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EmploymentStatusService } from 'src/app/core/services/employment-status.service';

@Component({
  selector: 'app-employment-status-list',
  templateUrl: './employment-status-list.component.html'
})
export class EmploymentStatusListComponent implements OnInit {
  editIcon = faEdit;
  archiveIcon = faTrashAlt;
  plusIcon = faPlusCircle;
  
  filtersForm: FormGroup;
  employmentStatuses: EmploymentStatus[];
  pagination: Pagination;
  employmentStatusParams: EmploymentStatusParams;

  constructor(
    private route: ActivatedRoute,
    private employmentStatusService: EmploymentStatusService,
    private alertify: AlertifyService,
    private fb: FormBuilder) { 
      this.employmentStatusParams = employmentStatusService.getEmploymentStatusParams();

      this.filtersForm = this.fb.group({
        name: ['', Validators.minLength(1)]
      });

      const nameControl = this.filtersForm.get('name');
      nameControl.valueChanges.pipe(
        debounceTime(600),
        distinctUntilChanged()
      ).subscribe((searchFor) => {
        const params = this.employmentStatusService.getEmploymentStatusParams();
        params.pageNumber = 1;
        params.name = searchFor;
        this.employmentStatusService.setEmploymentStatusParams(params);
        this.employmentStatusParams = params;
        this.loadEmploymentStatuses();
      });
    }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.employmentStatuses = data['employmentStatuses'].result;
      this.pagination = data['employmentStatuses'].pagination;
    });
  }

  loadEmploymentStatuses(): void {
    this.employmentStatusService.getPagedEmploymentStatuses()
      .subscribe({
        next: (res: PaginatedResult<EmploymentStatus[]>) => {
          this.employmentStatuses = res.result;
          this.pagination = res.pagination;
        },
        error: () => {
          this.alertify.error('Unable to load employment statuses');
        }
      });
  }

  pageChanged(event: any): void {
    const params = this.employmentStatusService.getEmploymentStatusParams();
    if (params.pageNumber !== event) {
      this.pagination.currentPage = event;
      params.pageNumber = event;
      this.employmentStatusService.setEmploymentStatusParams(params);
      this.employmentStatusParams = params;
      this.loadEmploymentStatuses();
    }
  }

  changeStatus(id: number): any {
    this.employmentStatusService.changeStatus(id).subscribe({
      next: () => {
        this.loadEmploymentStatuses();
        this.alertify.success('Status changed');
      },
      error: () => {
        this.alertify.error('Unable to archive');
      }
    });
  }

}
