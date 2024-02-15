import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { EmploymentStatus } from 'src/app/core/models/employmentStatus';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EmploymentStatusService } from 'src/app/core/services/employment-status.service';

@Component({
  selector: 'app-employment-status-create',
  templateUrl: './employment-status-create.component.html'
})
export class EmploymentStatusCreateComponent implements OnInit {
  createEmploymentStatusForm: UntypedFormGroup;
  employmentStatus: EmploymentStatus;

  constructor(
    private employmentStatusService: EmploymentStatusService,
    private alertify: AlertifyService,
    private fb: UntypedFormBuilder) { }

  ngOnInit(): void {
    this.createForm();
  }

  createForm(): void {
    this.createEmploymentStatusForm = this.fb.group({
      name: ['', Validators.required]
    });
  }

  create(): void {
    this.employmentStatus = Object.assign({}, this.createEmploymentStatusForm.value);
    this.employmentStatusService.createEmploymentStatus(this.employmentStatus).subscribe({
      next: () => {
        this.alertify.success('Successfully created');
        this.createEmploymentStatusForm.reset();
      },
      error: (error) => {
        this.alertify.error(error.error);
      }
    });
  }

}
