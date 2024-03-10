import { Component, OnInit } from '@angular/core';
import { FormControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
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
  employeeId: number;
  createEngagementForm: FormGroup;
  engagement: Engagement;

  existingEngagements: ExistingEngagement[];
  employee: Employee;
  workplaces: Workplace[];
  employmentStatuses: EmploymentStatus[];


  salary = new FormControl('', [
    Validators.required,
    Validators.min(300),
    Validators.max(5000)
  ]);

  dateFrom = new FormControl(null, [Validators.required]);
  dateTo = new FormControl(null, [Validators.required]);

  constructor(
    private route: ActivatedRoute,
    private engagementService: EngagementService,
    private employmentStatusService: EmploymentStatusService,
    private employeeService: EmployeeService,
    private workplaceService: WorkplaceService,
    private fb: FormBuilder,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.existingEngagements = data['employeeData'];
    });
    this.createForm();
    this.loadEmployee();
    this.loadWorkplaces();
    this.loadEmploymentStatuses();
  }

  createForm(): void {
    this.createEngagementForm = this.fb.group({
      workplaceId: [null, Validators.required],
      salary: this.salary,
      dateFrom: this.dateFrom,
      dateTo: this.dateTo,
      employmentStatusId: [null, Validators.required]
    });
  }


  loadEmployee(): void {
    const employeeId = parseInt(this.route.snapshot.paramMap.get('id'))
    this.employeeService.getEmployee(employeeId).subscribe({
        next: (res: any) => {
          this.employee = res;
        },
        error: () => {
          this.alertify.error('Unable get employee')
        }
      });
  }

  loadWorkplaces(): void {
    this.workplaceService.getWorkplaces().subscribe({
      next: (res: Workplace[]) => {
        this.workplaces = res;
      },
      error: () => {
        this.alertify.error('Unable to list workplaces');
      }
    });
  }

  loadEmploymentStatuses(): void {
    this.employmentStatusService.getEmploymentStatuses().subscribe({
      next: (res: EmploymentStatus[]) => {
        this.employmentStatuses = res;
      },
      error: () => {
        this.alertify.error('Unable to list Employment statuses');
      }
    })
  }

  loadEngagements(): void {
    this.engagementService.getEngagementForEmployee(this.employee.id).subscribe({
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
    this.engagementService.createEngagement(this.engagement).subscribe({
      next: () => {
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
