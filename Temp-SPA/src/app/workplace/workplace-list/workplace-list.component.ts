import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PaginatedResult, Pagination } from 'src/app/core/models/pagination';
import { Workplace } from 'src/app/core/models/workplace';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { WorkplaceService } from 'src/app/core/services/workplace.service';
import { faPenNib, faPenToSquare, faPlusCircle } from '@fortawesome/free-solid-svg-icons';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { debounceTime } from 'rxjs';


@Component({
  selector: 'app-workplace-list',
  templateUrl: './workplace-list.component.html'
})
export class WorkplaceListComponent implements OnInit {
  filtersForm: FormGroup;
  workplaces: Workplace[];
  pagination: Pagination;
  archiveIcon = faPenNib;
  editIcon = faPenToSquare
  plusIcon = faPlusCircle;
  workplaceParams: any = { name: '' };

  constructor(
    private route: ActivatedRoute,
    private workplaceService: WorkplaceService,
    private alertify: AlertifyService,
    private fb: FormBuilder) { 
      this.filtersForm = this.fb.group({
        name: ['', Validators.minLength(1)],
      })

      const nameControl = this.filtersForm.get('name');
      nameControl.valueChanges.pipe(
        debounceTime(1000)
      ).subscribe(() => {
        if (nameControl.value !== null) {
          this.workplaceParams.name = nameControl.value;
          this.loadWorkplaces();
        }
      });
    }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.workplaces = data['workplaces'].result;
      this.pagination = data['workplaces'].pagination;
    });
  }

  loadWorkplaces(): void {
    this.workplaceService.getPagedWorkplaces(this.pagination.currentPage, this.pagination.itemsPerPage)
      .subscribe({
        next: (res: PaginatedResult<Workplace[]>) => {
          this.workplaces = res.result;
          this.pagination = res.pagination;
        },
        error: (error) => {
          this.alertify.error(error.error);
        }
      });
  }

  resetFilters() {
    this.workplaceParams.name = '';
    this.filtersForm.patchValue({
      name: ''
    });
    this.loadWorkplaces();
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadWorkplaces();
  }

  changeStatus(id: number): void {
    this.workplaceService.changeStatus({id}).subscribe({
      next: () => {
        this.loadWorkplaces();
        this.alertify.success('Status is changed');
      },
      error: (error) => {
        this.alertify.error(error.error);
      }
    });
  }

}
