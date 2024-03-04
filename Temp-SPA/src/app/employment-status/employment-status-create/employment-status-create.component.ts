import { Component, OnInit } from '@angular/core';
import { FormControl, UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { EmploymentStatus } from 'src/app/core/models/employmentStatus';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EmploymentStatusService } from 'src/app/core/services/employment-status.service';
import { EmploymentStatusValidators } from '../employment-status-validators';

@Component({
  selector: 'app-employment-status-create',
  templateUrl: './employment-status-create.component.html'
})
export class EmploymentStatusCreateComponent implements OnInit {
  createEmploymentStatusForm: UntypedFormGroup;
  employmentStatus: EmploymentStatus;

  name = new FormControl('',[
    Validators.required,
    Validators.minLength(3),
    Validators.maxLength(60)],
    [this.validators.validateNameNotTaken()]);

  constructor(
    private employmentStatusService: EmploymentStatusService,
    private alertify: AlertifyService,
    private fb: UntypedFormBuilder,
    private validators: EmploymentStatusValidators) { }

  ngOnInit(): void {
    this.createEmploymentStatusForm = this.fb.group({
      name: this.name
    });
  }

  create(): void {
    this.employmentStatus = { ...this.createEmploymentStatusForm.value };
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
