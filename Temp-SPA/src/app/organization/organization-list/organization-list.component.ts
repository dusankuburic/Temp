import { AfterViewInit, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faEdit, faPlusCircle, faProjectDiagram, faTrashAlt, faUsers } from '@fortawesome/free-solid-svg-icons';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { Subscription, debounceTime, distinctUntilChanged } from 'rxjs';
import { Organization, OrganizationParams } from 'src/app/core/models/organization';
import { PaginatedResult, Pagination } from 'src/app/core/models/pagination';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { OrganizationService } from 'src/app/core/services/organization.service';
import { SelectionOption } from 'src/app/shared/components/tmp-select/tmp-select.component';
import { OrganizationCreateModalComponent } from '../organization-create-modal/organization-create-modal.component';
import { OrganizationEditModalComponent } from '../organization-edit-modal/organization-edit-modal.component';

@Component({
  selector: 'app-organization-list',
  templateUrl: './organization-list.component.html'
})
export class OrganizationListComponent implements OnInit, AfterViewInit {
  editOrganizationIcon = faEdit
  archiveOrganizationIcon = faTrashAlt
  innerGroupsIcon = faUsers
  createGroupIcon = faProjectDiagram
  plusIcon = faPlusCircle;

  bsModalRef?: BsModalRef;
  subscriptions: Subscription;
  filtersForm: FormGroup;
  organizations: Organization[];
  pagination: Pagination;
  organizationParams: OrganizationParams;
  groupsSelect: SelectionOption[] = [
    {value: '', display: 'Select with groups', disabled: true },
    {value: 'all', display: 'All'},
    {value: 'yes', display: 'With groups'},
    {value: 'no', display: 'Without groups'}
  ];

  constructor(
    private route: ActivatedRoute,
    private organizationsService: OrganizationService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private bsModalService: BsModalService) { 
      this.organizationParams = organizationsService.getOrganizationParams();

      this.filtersForm = this.fb.group({
        withGroups: [''],
        name: ['']
      })
    }
    
  ngAfterViewInit(): void {
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

  openCreateModal(): void {
    const initialState: ModalOptions = {
      class: 'modal-dialog-centered',
      initialState: {
        title: 'Create Organization'
      }
    };
    this.subscriptions = new Subscription();
    this.bsModalRef = this.bsModalService.show(OrganizationCreateModalComponent, initialState);
    if (this.bsModalRef?.onHidden) {
      this.subscriptions.add(this.bsModalRef.onHidden.subscribe(() => {
        if (this.bsModalRef.content.isSaved)
          this.loadOrganizations();
        
        this.unsubscribe();
      }))
    }
  }

  openEditModal(id: number): void {
    const initialState: ModalOptions = {
      class: 'modal-dialog-centered',
      initialState: {
        title: 'Edit Organization',
        organizationId: id
      }
    };
    this.subscriptions = new Subscription();
    this.bsModalRef = this.bsModalService.show(OrganizationEditModalComponent, initialState);
    if (this.bsModalRef?.onHidden) {
      this.subscriptions.add(this.bsModalRef.onHidden.subscribe(() => {
        if (this.bsModalRef.content.isSaved)
          this.loadOrganizations();
        
          this.unsubscribe();
      }))
    }
  }

  unsubscribe() {
    this.subscriptions.unsubscribe();
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