import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PaginatedResult, Pagination } from 'src/app/core/models/pagination';
import { Workplace, WorkplaceParams } from 'src/app/core/models/workplace';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { WorkplaceService } from 'src/app/core/services/workplace.service';
import { faPenToSquare, faPlusCircle, faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { FormBuilder, FormGroup } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';

@Component({
  selector: 'app-workplace-list',
  templateUrl: './workplace-list.component.html'
})
export class WorkplaceListComponent implements OnInit {
  archiveIcon = faTrashAlt;
  editIcon = faPenToSquare
  plusIcon = faPlusCircle;

  filtersForm: FormGroup;
  workplaces: Workplace[];
  pagination: Pagination;
  workplaceParams: WorkplaceParams;

  constructor(
    private route: ActivatedRoute,
    private workplaceService: WorkplaceService,
    private alertify: AlertifyService,
    private fb: FormBuilder) { 
      this.workplaceParams = workplaceService.getWorkplaceParams();

      this.filtersForm = this.fb.group({
        name: [''],
      })

      const nameControl = this.filtersForm.get('name');
      nameControl.valueChanges.pipe(
        debounceTime(600),
        distinctUntilChanged(),
      ).subscribe((searchFor) => {
        const params = this.workplaceService.getWorkplaceParams();
        params.pageNumber = 1;
        params.name = searchFor;
        this.workplaceService.setWorkplaceParams(params);
        this.workplaceParams = params;
        this.loadWorkplaces();
      });
    }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.workplaces = data['workplaces'].result;
      this.pagination = data['workplaces'].pagination;
    });
  }

  loadWorkplaces(): void {
    this.workplaceService.getPagedWorkplaces()
      .subscribe({
        next: (res: PaginatedResult<Workplace[]>) => {
          this.workplaces = res.result;
          this.pagination = res.pagination;
        },
        error: () => {
          this.alertify.error('Unable to load workplaces');
        }
      });
  }

  pageChanged(event: any): void {
    const params = this.workplaceService.getWorkplaceParams();
    if (params.pageNumber !== event) {
      this.pagination.currentPage = event;
      params.pageNumber = event;
      this.workplaceService.setWorkplaceParams(params);
      this.workplaceParams = params;
      this.loadWorkplaces();
    }
  }

  changeStatus(id: number): void {
    this.workplaceService.changeStatus({id}).subscribe({
      next: () => {
        this.loadWorkplaces();
        this.alertify.success('Status is changed');
      },
      error: () => {
        this.alertify.error('Unable to archive');
      }
    });
  }

}
