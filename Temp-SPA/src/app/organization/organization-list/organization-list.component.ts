import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faEdit, faPenNib, faPlusCircle, faProjectDiagram, faUsers } from '@fortawesome/free-solid-svg-icons';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { Organization, OrganizationParams } from 'src/app/core/models/organization';
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
  plusIcon = faPlusCircle;

  filtersForm: FormGroup;
  organizations: Organization[];
  pagination: Pagination;
  organizationParams: OrganizationParams;
  groupsSelect = [
    {value: '', display: 'Select with groups', disabled: true },
    {value: 'all', display: 'All', disabled: false},
    {value: 'yes', display: 'With groups', disabled: false},
    {value: 'no', display: 'Without groups', disabled: false}
  ];

  constructor(
    private route: ActivatedRoute,
    private organizationsService: OrganizationService,
    private alertify: AlertifyService,
    private fb: FormBuilder) { 
      this.organizationParams = organizationsService.getOrganizationParams();

      this.filtersForm = this.fb.group({
        withGroups: ['', Validators.minLength(1)],
        name: ['', Validators.minLength(1)]
      })

      const withGroupsControl = this.filtersForm.get('withGroups');
      withGroupsControl.valueChanges.pipe(
        debounceTime(100),
        distinctUntilChanged()
      ).subscribe((searchFor) => {
        const params = this.organizationsService.getOrganizationParams();
        params.pageNumber = 1;
        this.organizationParams.withGroups = searchFor;
        this.organizationsService.setOrganizationParams(params);
        this.organizationParams = params;
        this.loadOrganizations();
      })

      const nameControl = this.filtersForm.get('name');
      nameControl.valueChanges.pipe(
        debounceTime(600),
        distinctUntilChanged()
      ).subscribe((searchFor) => {
        const params = this.organizationsService.getOrganizationParams();
        params.pageNumber = 1;
        params.name = searchFor;
        this.organizationsService.setOrganizationParams(params);
        this.organizationParams = params;
        this.loadOrganizations();
      });
    }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.organizations = data['organizations'].result;
      this.pagination = data['organizations'].pagination;
    });
  }

  loadOrganizations(): void {
    this.organizationsService.getPagedOrganizations()
      .subscribe({
        next: (res: PaginatedResult<Organization[]>) => {
          this.organizations = res.result;
          this.pagination = res.pagination;
        },
        error: () => {
          this.alertify.error('Unable to load organizations');
        }
      })
  }

  pageChanged(event: any): void {
    const params = this.organizationsService.getOrganizationParams();
    if (params.pageNumber !== event) {
      this.pagination.currentPage = event;
      params.pageNumber = event;
      this.organizationsService.setOrganizationParams(params);
      this.organizationParams = params;
      this.loadOrganizations();
    }
  }

  changeStatus(id: number): void {
    this.organizationsService.changeStatus(id).subscribe({
      next: () => {
        this.loadOrganizations();
        this.alertify.success('Status changed');
      },
      error: () => {
        this.alertify.error('Unable to archive');
      }
    });
  }

}