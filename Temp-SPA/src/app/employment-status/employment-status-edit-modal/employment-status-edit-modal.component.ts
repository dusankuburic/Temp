import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { takeUntil } from 'rxjs';
import { EmploymentStatus } from 'src/app/core/models/employmentStatus';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EmploymentStatusService } from 'src/app/core/services/employment-status.service';
import { EmploymentStatusValidators } from '../employment-status-validators';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { DestroyableComponent } from 'src/app/core/base/destroyable.component';

import { faTimes } from '@fortawesome/free-solid-svg-icons';

@Component({
    selector: 'app-employment-status-edit-modal',
    templateUrl: './employment-status-edit-modal.component.html',
    standalone: false
})
export class EmploymentStatusEditModalComponent extends DestroyableComponent implements OnInit {
  faTimes = faTimes;
  editEmploymentStatusForm!: FormGroup;
  employmentStatus!: EmploymentStatus;
  title?: string;
  employmentStatusId!: number;

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
    public bsModalRef: BsModalRef) {
      super();
    }

  ngOnInit(): void {
    this.editEmploymentStatusForm = this.fb.group({
      name: this.name
    });

    this.employmentStatusService.getEmploymentStatus(this.employmentStatusId).pipe(takeUntil(this.destroy$)).subscribe({
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
    this.employmentStatusService.updateEmploymentStatus(employmentStatusForm).pipe(takeUntil(this.destroy$)).subscribe({
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
