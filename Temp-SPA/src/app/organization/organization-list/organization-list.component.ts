import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faEdit, faPenNib, faProjectDiagram, faUsers } from '@fortawesome/free-solid-svg-icons';
import { Organization } from 'src/app/core/models/organization';
import { PaginatedResult, Pagination } from 'src/app/core/models/pagination';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { OrganizationService } from 'src/app/core/services/organization.service';

@Component({
  selector: 'app-organization-list',
  templateUrl: './organization-list.component.html'
})
export class OrganizationListComponent implements OnInit {
  editOrganizationIcon = faEdit
  archiveOrganizationIcon = faPenNib
  innerGroupsIcon = faUsers
  createGroupIcon = faProjectDiagram

  organizations: Organization[];
  pagination: Pagination;

  constructor(
    private route: ActivatedRoute,
    private organizationsService: OrganizationService,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.organizations = data['organizations'].result;
      this.pagination = data['organizations'].pagination;
    });
  }

  loadOrganizations(): void {
    this.organizationsService.getPagedOrganizations(this.pagination.currentPage, this.pagination.itemsPerPage)
      .subscribe({
        next: (res: PaginatedResult<Organization[]>) => {
          this.organizations = res.result;
          this.pagination = res.pagination;
        },
        error: (error) => {
          this.alertify.error('Unable to load organizations');
        }
      })
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadOrganizations();
  }

  changeStatus(id: number): void {
    this.organizationsService.changeStatus(id).subscribe({
      next: () => {
        this.loadOrganizations();
        this.alertify.success('Status changed');
      },
      error: (error) => {
        this.alertify.error(error.error);
      }
    });
  }

}
