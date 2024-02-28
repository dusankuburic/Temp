import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { Employee } from 'src/app/core/models/employee';
import { EmploymentStatus } from 'src/app/core/models/employmentStatus';
import { Engagement, ExistingEngagement } from 'src/app/core/models/engagement';
import { Workplace } from 'src/app/core/models/workplace';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EmployeeService } from 'src/app/core/services/employee.service';
import { EmploymentStatusService } from 'src/app/core/services/employment-status.service';
import { EngagementService } from 'src/app/core/services/engagement.service';
import { WorkplaceService } from 'src/app/core/services/workplace.service';

@Component({
  selector: 'app-engagement-create',
  templateUrl: './engagement-create.component.html'
})
export class EngagementCreateComponent implements OnInit {
  employeeData: any;
  employeeId: number;
  createEngagementForm: UntypedFormGroup;
  engagement: Engagement;
  bsConfig: Partial<BsDatepickerConfig>;

  existingEngagements: ExistingEngagement[];
  employee: Employee;
  workplaces: Workplace[];
  employmentStatuses: EmploymentStatus[];

  constructor(
    private route: ActivatedRoute,
    private engagementService: EngagementService,
    private employmentStatusService: EmploymentStatusService,
    private employeeService: EmployeeService,
    private workplaceService: WorkplaceService,
    private fb: UntypedFormBuilder,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.bsConfig = {
      containerClass: 'theme-dark-blue'
    },
    this.route.data.subscribe(data => {
      this.existingEngagements = data['employeeData'];
    });
    this.createForm();
    this.loadEmployee();
    this.loadWorkplaces();
    this.loadEmploymentStatuses();
  }

  loadEmployee(): void {
    //this.employeeData.Employee.Id
    this.employeeService.getEmployee(6).subscribe({
        next: (res: any) => {
          this.employee = res;
        },
        error: () => {
          this.alertify.error('Problem retrieving data')
        }
      });
  }

  loadWorkplaces(): void {
    this.workplaceService.getWorkplaces().subscribe({
      next: (res: Workplace[]) => {
        this.workplaces = res;
      },
      error: () => {
        this.alertify.error('Problem retrieving workplaces');
      }
    });
  }

  loadEmploymentStatuses(): void {
    this.employmentStatusService.getEmploymentStatuses().subscribe({
      next: (res: EmploymentStatus[]) => {
        this.employmentStatuses = res;
      },
      error: () => {
        this.alertify.error('Problem retrieving Employment Statuses');
      }
    })
  }

  loadEngagements(): void {
    this.engagementService.getEngagementForEmployee(this.employeeData.Employee.Id).subscribe({
      next: (res: any) => {
        this.employeeData = res;
      },
      error: () => {
        this.alertify.error('Problem retrieving data');
      }
    });
  }

  createForm(): void {
    this.createEngagementForm = this.fb.group({
      workplaceId: [null, Validators.required],
      salary: ['', [Validators.required, Validators.min(300), Validators.max(5000)]],
      dateFrom: [null, Validators.required],
      dateTo: [null, Validators.required],
      employmentStatusId: [null, Validators.required]
    });
  }

  create(): void {
    this.engagement = { ...this.createEngagementForm.value, id: this.employeeData.Employee.Id };
    this.engagementService.createEngagement(this.engagement).subscribe({
      next: () => {
        this.loadEngagements();
        this.alertify.success('Successfully created');
        this.createEngagementForm.reset();
      },
      error: (error) => {
        this.alertify.error(error.error);
      }
    });
  }

}
