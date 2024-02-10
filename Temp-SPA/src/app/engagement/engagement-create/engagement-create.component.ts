import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { Employee } from 'src/app/models/employee';
import { EmploymentStatus } from 'src/app/models/employmentStatus';
import { Engagement, ExistingEngagement } from 'src/app/models/engagement';
import { Workplace } from 'src/app/models/workplace';
import { AlertifyService } from 'src/app/services/alertify.service';
import { EmployeeService } from 'src/app/services/employee.service';
import { EngagementService } from 'src/app/services/engagement.service';
import { WorkplaceService } from 'src/app/services/workplace.service';

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
    this.loadWorkplaces();
  }

  loadEmployee(): void {
    this.employeeService.getEmployee(this.employeeData.Employee.Id).subscribe((res: any) => {
      this.employee = res;
      console.log(res);
    }, error => {
      this.alertify.error('Problem retrieving data');
    })
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

  //INFO: not paged
  loadEmploymentStatuses(): void {

  }

  loadEngagements(): void {
    this.engagementService.getEngagementForEmployee(this.employeeData.Employee.Id).subscribe((res: any) => {
      this.employeeData = res;
    }, error => {
      this.alertify.error('Problem retrieving data');
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

  create(): any {
    this.engagement = Object.assign({}, this.createEngagementForm.value);
    this.engagement.employeeId = this.employeeData.Employee.Id;

    this.engagementService.createEngagement(this.engagement).subscribe(() => {
      this.loadEngagements();
      console.log(this.employeeData);
      this.alertify.success('Successfully created');
      this.createEngagementForm.reset();
    }, error => {
      this.alertify.error(error.error);
    });


  }

}
