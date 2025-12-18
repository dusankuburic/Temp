import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { takeUntil } from 'rxjs';
import { Employee } from 'src/app/core/models/employee';
import { Engagement, ExistingEngagement } from 'src/app/core/models/engagement';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EmployeeService } from 'src/app/core/services/employee.service';
import { EmploymentStatusService } from 'src/app/core/services/employment-status.service';
import { EngagementService } from 'src/app/core/services/engagement.service';
import { WorkplaceService } from 'src/app/core/services/workplace.service';
import { SelectionOption } from 'src/app/shared/components/tmp-select/tmp-select.component';
import { DestroyableComponent } from 'src/app/core/base/destroyable.component';

@Component({
  selector: 'app-engagement-create-modal',
  templateUrl: './engagement-create-modal.component.html'
})
export class EngagementCreateModalComponent extends DestroyableComponent implements OnInit{
  

  title?: string;
  employeeId: number;
  createEngagementForm: FormGroup;
  engagement: Engagement;

  existingEngagements: ExistingEngagement[];
  employee: Employee;
  workplacesList: SelectionOption[];
  employmentStatusesList: SelectionOption[];

  salary = new FormControl('', [
    Validators.required,
    Validators.min(300),
    Validators.max(5000)
  ]);

  dateFrom = new FormControl(null, [Validators.required]);
  dateTo = new FormControl(null, [Validators.required]);

  constructor(
    private engagementService: EngagementService,
    private employmentStatusService: EmploymentStatusService,
    private employeeService: EmployeeService,
    private workplaceService: WorkplaceService,
    private fb: FormBuilder,
    private alertify: AlertifyService,
    public bsModalRef: BsModalRef) {
    super();
  }

  ngOnInit(): void {
    this.createEngagementForm = this.fb.group({
      workplaceId: [null, Validators.required],
      salary: this.salary,
      dateFrom: this.dateFrom,
      dateTo: this.dateTo,
      employmentStatusId: [null, Validators.required]
    });

    this.engagementService.getEngagementForEmployee(this.employeeId).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res) => {
        this.existingEngagements = res;
      },
      error: () => {
        this.alertify.error('Unable to get employee data');
      }
    });

    this.loadEmployee();
    this.loadWorkplaces();
    this.loadEmploymentStatuses();
  }

  loadEmployee(): void {
    this.employeeService.getEmployee(this.employeeId).pipe(takeUntil(this.destroy$)).subscribe({
        next: (res: any) => {
          this.employee = res;
        },
        error: () => {
          this.alertify.error('Unable get employee')
        }
      });
  }

  loadWorkplaces(): void {
    this.workplaceService.getWorkplacesForSelect().pipe(takeUntil(this.destroy$)).subscribe({
      next: (res) => {
        this.workplacesList = [
          {value: null, display: 'Select Workplace', disabled: true},
          ...res
        ];
      },
      error: () => {
        this.alertify.error('Unable to list workplaces');
      }
    });
  }

  loadEmploymentStatuses(): void {
    this.employmentStatusService.getEmploymentStatusesForSelect().pipe(takeUntil(this.destroy$)).subscribe({
      next: (res) => {
        this.employmentStatusesList = [
          {value: null, display: 'Select Team', disabled: true},
          ...res
        ];
      },
      error: () => {
        this.alertify.error('Unable to list Employment statuses');
      }
    })
  }

  loadEngagements(): void {
    this.engagementService.getEngagementForEmployee(this.employee.id).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res: any) => {
        this.existingEngagements = res;
      },
      error: () => {
        this.alertify.error('Unable to list engagements');
      }
    });
  }

  create(): void {
    this.engagement = { ...this.createEngagementForm.value, employeeId: this.employee.id };
    this.engagementService.createEngagement(this.engagement).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.bsModalRef.content.isSaved = true;
        this.loadEngagements();
        this.alertify.success('Successfully created');
        this.createEngagementForm.reset();
      },
      error: () => {
        this.alertify.error('Unable to create engagement');
      }
    });
  } 
}
