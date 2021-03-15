import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { Engagement } from 'src/app/_models/engagement';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { EngagementService } from 'src/app/_services/engagement.service';

@Component({
  selector: 'app-engagement-create',
  templateUrl: './engagement-create.component.html'
})
export class EngagementCreateComponent implements OnInit {
  employeeData: any;
  createEngagementForm: FormGroup;
  engagement: Engagement;
  bsConfig: Partial<BsDatepickerConfig>;

  constructor(
    private route: ActivatedRoute,
    private engagementService: EngagementService,
    private fb: FormBuilder,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.bsConfig = {
      containerClass: 'theme-dark-blue'
    },
    this.route.data.subscribe(data => {
      this.employeeData = data['employeeData'];
    });
    this.createForm();
  }

  loadEngagements(): void {
    this.engagementService.getEngagementForEmployee(this.employeeData.Employee.Id).subscribe((res: any) => {
      this.employeeData = res;
    }, error => {
      this.alertify.error('Problem retriving data');
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
      this.alertify.success('Successfully created');
      this.createEngagementForm.reset();
    }, error => {
      this.alertify.error(error.error);
    });

  }

}
