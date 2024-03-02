import { Component, OnInit } from '@angular/core';
import { FormControl, UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { EmploymentStatus } from 'src/app/core/models/employmentStatus';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EmploymentStatusService } from 'src/app/core/services/employment-status.service';

@Component({
  selector: 'app-employment-status-edit',
  templateUrl: './employment-status-edit.component.html'
})
export class EmploymentStatusEditComponent implements OnInit {
  editEmploymentStatusForm: UntypedFormGroup;
  employmentStatus: EmploymentStatus;

  constructor(
    private employmentStatusService: EmploymentStatusService,
    private route: ActivatedRoute,
    private fb: UntypedFormBuilder,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.employmentStatus = data['employmentStatus'];
    });
    this.createForm();
  }

  name = new FormControl('', [
    Validators.required,
    Validators.minLength(3),
    Validators.maxLength(60)
  ])

  createForm(): void {
    this.editEmploymentStatusForm = this.fb.group({
      name: this.name.setValue(this.employmentStatus.name)
    });
  }

  update(): void {
    const employmentStatusForm = { ...this.editEmploymentStatusForm.value, id: this.employmentStatus.id};
    this.employmentStatusService.updateEmploymentStatus(employmentStatusForm).subscribe({
      next: () => {
        this.alertify.success('Successfully updated');
      },
      error: (error) => {
        this.alertify.error(error.error);
      }
    });
  }

}
