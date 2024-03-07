import { Component, OnInit } from '@angular/core';
import { FormControl, UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { EmploymentStatus } from 'src/app/core/models/employmentStatus';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EmploymentStatusService } from 'src/app/core/services/employment-status.service';
import { EmploymentStatusValidators } from '../employment-status-validators';

@Component({
  selector: 'app-employment-status-edit',
  templateUrl: './employment-status-edit.component.html'
})
export class EmploymentStatusEditComponent implements OnInit {
  editEmploymentStatusForm: UntypedFormGroup;
  employmentStatus: EmploymentStatus;

  name = new FormControl('', [
    Validators.required,
    Validators.minLength(3),
    Validators.maxLength(60)
  ])

  constructor(
    private employmentStatusService: EmploymentStatusService,
    private route: ActivatedRoute,
    private fb: UntypedFormBuilder,
    private alertify: AlertifyService,
    private validators: EmploymentStatusValidators) { }

  ngOnInit(): void {
    this.editEmploymentStatusForm = this.fb.group({
      name: this.name
    });

    this.route.data.subscribe(data => {
      this.employmentStatus = data['employmentStatus'];
    });
    this.setupForm(this.employmentStatus);
  }


  setupForm(employmentStatus: EmploymentStatus): void {
    if (this.editEmploymentStatusForm)
      this.editEmploymentStatusForm.reset();

      this.name.addAsyncValidators(this.validators.validateNameNotTaken(employmentStatus.name));

      this.editEmploymentStatusForm.patchValue({
        name: employmentStatus.name
      });
  }

  update(): void {
    const employmentStatusForm = { ...this.editEmploymentStatusForm.value, id: this.employmentStatus.id};
    this.employmentStatusService.updateEmploymentStatus(employmentStatusForm).subscribe({
      next: () => {
        this.alertify.success('Successfully updated');
      },
      error: () => {
        this.alertify.error('Unable to update engagement');
      }
    });
  }

}
