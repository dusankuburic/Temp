import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { Engagement } from 'src/app/_models/engagement';
import { EngagementService } from 'src/app/_services/engagement.service';

@Component({
  selector: 'app-engagement-create',
  templateUrl: './engagement-create.component.html',
  styleUrls: ['./engagement-create.component.scss']
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
    private router: Router) { }

  ngOnInit(): void {
    this.bsConfig = {
      containerClass: 'theme-dark-blue'
    },
    this.route.data.subscribe(data => {
      this.employeeData = data['employeeData'];
    });
    this.createForm();
  }

  createForm(): void {
    this.createEngagementForm = this.fb.group({
      workplaceId: [null, Validators.required],
      dateFrom: [null, Validators.required],
      dateTo: [null, Validators.required],
      employmentStatusId: [null, Validators.required]
    });
  }

  create(): any {
    this.engagement = Object.assign({}, this.createEngagementForm.value);

    console.log(this.engagement);
  }

}
