import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { EmploymentStatus } from 'src/app/core/models/employmentStatus';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EmploymentStatusService } from 'src/app/core/services/employment-status.service';
import { EmploymentStatusValidators } from '../employment-status-validators';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-employment-status-create-modal',
  templateUrl: './employment-status-create-modal.component.html'
})
export class EmploymentStatusCreateModalComponent implements OnInit {
  createEmploymentStatusForm: FormGroup;
  employmentStatus: EmploymentStatus;
  title?: string;

  name = new FormControl('',[
    Validators.required,
    Validators.minLength(3),
    Validators.maxLength(60)],
    [this.validators.validateNameNotTaken()]);

  constructor(
    private employmentStatusService: EmploymentStatusService,
    private fb: FormBuilder,
    private alertify: AlertifyService,
    private validators: EmploymentStatusValidators,
    public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
    this.createEmploymentStatusForm = this.fb.group({
      name: this.name
    });
  }

  create(): void {
    this.employmentStatus = { ...this.createEmploymentStatusForm.value };
    this.employmentStatusService.createEmploymentStatus(this.employmentStatus).subscribe({
      next: () => {
        this.bsModalRef.content.isSaved = true;
        this.alertify.success('Successfully created');
        this.createEmploymentStatusForm.reset();
      },
      error: () => {
        this.alertify.error('Unable to create employment status');
      }
    });
  }
}
