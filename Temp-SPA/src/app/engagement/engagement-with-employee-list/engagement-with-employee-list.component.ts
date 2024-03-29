import { AfterViewInit, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faCodeBranch } from '@fortawesome/free-solid-svg-icons';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { Subscription, debounceTime, distinctUntilChanged } from 'rxjs';
import { Employee } from 'src/app/core/models/employee';
import { EngagementParams } from 'src/app/core/models/engagement';
import { PaginatedResult, Pagination } from 'src/app/core/models/pagination';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EngagementService } from 'src/app/core/services/engagement.service';
import { SelectionOption } from 'src/app/shared/components/tmp-select/tmp-select.component';
import { EngagementCreateModalComponent } from '../engagement-create-modal/engagement-create-modal.component';

@Component({
  selector: 'app-engagement-with-employee-list',
  templateUrl: './engagement-with-employee-list.component.html'
})
export class EngagementWithEmployeeListComponent implements OnInit, AfterViewInit {
  addEngagementIcon = faCodeBranch

  bsModalRef?: BsModalRef;
  subscriptions: Subscription;
  filtersForm: FormGroup;
  rolesSelect: SelectionOption[] = [
    {value: '', display: 'Select Role', disabled: true},
    {value: '', display: 'All'},
    {value: 'User', display: 'User'},
    {value: 'Admin', display: 'Admin'},
    {value: 'Moderator', display: 'Moderator'},
    {value: 'None', display: 'None'}];
  employees: Employee[];
  pagination: Pagination;
  engagementParams: EngagementParams;

  constructor(
    private route: ActivatedRoute,
    private engagementService: EngagementService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private bsModalService: BsModalService) { 
      this.engagementParams = engagementService.getEngagementParams();

      this.filtersForm = this.fb.group({
        role: [''],
        firstName: [''],
        lastName: [''],
      });

    }
    
  ngAfterViewInit(): void {
    
    const roleControl = this.filtersForm.get('role');
    roleControl.valueChanges.pipe(
      debounceTime(100),
      distinctUntilChanged()
    ).subscribe((searchFor) => {
      const params = this.engagementService.getEngagementParams();
      params.pageNumber = 1;
      this.engagementParams.role = searchFor;
      this.engagementService.setEngagementParams(params);
      this.engagementParams = params;
      this.loadEmployeesWithEngagement();
    });

    const firstNameControl = this.filtersForm.get('firstName');
    firstNameControl.valueChanges.pipe(
      debounceTime(600),
      distinctUntilChanged()
    ).subscribe((searchFor) => {
      const params = this.engagementService.getEngagementParams();
      params.pageNumber = 1;
      params.firstName = searchFor;
      this.engagementService.setEngagementParams(params);
      this.engagementParams = params;
      this.loadEmployeesWithEngagement();
    });

    const lastNameControl = this.filtersForm.get('lastName');
    lastNameControl.valueChanges.pipe(
      debounceTime(600),
      distinctUntilChanged()
    ).subscribe((searchFor) => {
      const params = this.engagementService.getEngagementParams();
      params.pageNumber = 1;
      params.lastName = searchFor;
      this.engagementService.setEngagementParams(params);
      this.engagementParams = params;
      this.loadEmployeesWithEngagement();
    });
  }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.employees = data['employeesWith'].result;
      this.pagination = data['employeesWith'].pagination;
    });
  }

  openCreateModal(employeeId: number): void {
    const initialState: ModalOptions = {
      class: 'modal-dialog-centered modal-lg',
      initialState: {
        title: 'Create Engagement',
        employeeId: employeeId
      }
    };
    this.subscriptions = new Subscription();
    this.bsModalRef = this.bsModalService.show(EngagementCreateModalComponent, initialState);
    if (this.bsModalRef?.onHidden) {
      this.subscriptions.add(this.bsModalRef.onHidden.subscribe(() => {
        if (this.bsModalRef.content.isSaved)
          this.loadEmployeesWithEngagement();

        this.unsubscribe();
      }))
    }
  }

  unsubscribe() {
    this.subscriptions.unsubscribe();
  }

  loadEmployeesWithEngagement(): void {
    this.engagementService.getEmployeesWithEngagement().subscribe({
        next: (res: PaginatedResult<Employee[]>) => {
          this.employees = res.result;
          this.pagination = res.pagination;
        },
        error: () => {
          this.alertify.error('Unable to list employees');
        }
      });
  }

  pageChanged(event: any): void {
    const params = this.engagementService.getEngagementParams();
    if (params.pageNumber !== event) {
      this.pagination.currentPage = event;
      params.pageNumber = event;
      this.engagementService.setEngagementParams(params);
      this.engagementParams = params;
      this.loadEmployeesWithEngagement();
    }
  }

}
