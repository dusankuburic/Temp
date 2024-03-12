import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { EmploymentStatus } from 'src/app/core/models/employmentStatus';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EmploymentStatusService } from 'src/app/core/services/employment-status.service';
import { EmploymentStatusValidators } from '../employment-status-validators';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-employment-status-edit-modal',
  templateUrl: './employment-status-edit-modal.component.html'
})
export class EmploymentStatusEditModalComponent implements OnInit {
  editEmploymentStatusForm: FormGroup;
  employmentStatus: EmploymentStatus;
  title?: string;
  employmentStatusId: number;

  name = new FormControl('', [
    Validators.required,
    Validators.minLength(3),
    Validators.maxLength(60)
  ])

  constructor(
    private employmentStatusService: EmploymentStatusService,
    private fb: FormBuilder,
    private alertify: AlertifyService,
    private validators: EmploymentStatusValidators,
    public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
    this.editEmploymentStatusForm = this.fb.group({
      name: this.name
    });

    this.employmentStatusService.getEmploymentStatus(this.employmentStatusId).subscribe({
      next: (res) => {
        this.employmentStatus = res;
        this.setupForm(this.employmentStatus);
      },
      error: () => {
        this.alertify.error('Unable to get employment status');
      }
    })
  }

  setupForm(employmentStatus: EmploymentStatus): void {
    this.editEmploymentStatusForm.patchValue({
      name: employmentStatus.name
    });

    this.name.addAsyncValidators(this.validators.validateNameNotTaken(employmentStatus.name));
  }

  update(): void {
    const employmentStatusForm = { ...this.editEmploymentStatusForm.value, id: this.employmentStatus.id};
    this.employmentStatusService.updateEmploymentStatus(employmentStatusForm).subscribe({
      next: () => {
        this.bsModalRef.content.isSaved = true;
        this.alertify.success('Successfully updated');
      },
      error: () => {
        this.alertify.error('Unable to update engagement');
      }
    });
  }
}
