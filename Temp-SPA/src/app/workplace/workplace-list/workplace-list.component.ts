import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { Workplace } from 'src/app/_models/workplace';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { WorkplaceService } from 'src/app/_services/workplace.service';

@Component({
  selector: 'app-workplace-list',
  templateUrl: './workplace-list.component.html'
})
export class WorkplaceListComponent implements OnInit {
  workplaces: Workplace[];
  pagination: Pagination;

  constructor(
    private route: ActivatedRoute,
    private workplaceService: WorkplaceService,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.workplaces = data['workplaces'].result;
      this.pagination = data['workplaces'].pagination;
    });
  }

  loadWorkplaces(): void {
    this.workplaceService.getWorkplaces(this.pagination.currentPage, this.pagination.itemsPerPage)
      .subscribe((res: PaginatedResult<Workplace[]>) => {
        this.workplaces = res.result;
        this.pagination = res.pagination;
      }, error => {
        this.alertify.error(error.error);
      })
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadWorkplaces();
  }

  changeStatus(id: number) {
    this.workplaceService.changeStatus(id).subscribe(() => {
      this.loadWorkplaces();
      this.alertify.success('Status change');
    }, error => {
      this.alertify.error(error.error);
    });
  }


}
