import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faEdit, faPenNib } from '@fortawesome/free-solid-svg-icons';
import { debounceTime } from 'rxjs';
import { EmploymentStatus } from 'src/app/core/models/employmentStatus';
import { PaginatedResult, Pagination } from 'src/app/core/models/pagination';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EmploymentStatusService } from 'src/app/core/services/employment-status.service';

@Component({
  selector: 'app-employment-status-list',
  templateUrl: './employment-status-list.component.html'
})
export class EmploymentStatusListComponent implements OnInit {
  filtersForm: FormGroup;
  editIcon = faEdit
  archiveIcon = faPenNib

  employmentStatuses: EmploymentStatus[];
  pagination: Pagination;
  employmentStatusParams: any = { name: '' };

  constructor(
    private route: ActivatedRoute,
    private employmentStatusService: EmploymentStatusService,
    private alertify: AlertifyService,
    private fb: FormBuilder) { 
      this.filtersForm = this.fb.group({
        name: ['', Validators.minLength(1)]
      });

      const nameControl = this.filtersForm.get('name');
      nameControl.valueChanges.pipe(
        debounceTime(1000)
      ).subscribe(() => {
        if (nameControl.value !== null) {
          this.employmentStatusParams.name = nameControl.value;
          this.loadEmploymentStatuses();
        }
      });
    }
    
  resetFilters() {
    this.employmentStatusParams.name = '';
    this.filtersForm.patchValue({
      name: ''
    });
    this.loadEmploymentStatuses();
  }

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
