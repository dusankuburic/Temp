import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { EmploymentStatus } from 'src/app/models/employmentStatus';
import { AlertifyService } from 'src/app/services/alertify.service';
import { EmploymentStatusService } from 'src/app/services/employment-status.service';

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

  createForm(): void {
    this.editEmploymentStatusForm = this.fb.group({
      name: [this.employmentStatus.name, Validators.required]
    });
  }

  update(): any {
    const employmentStatusForm = Object.assign({}, this.editEmploymentStatusForm.value);
    this.employmentStatusService.updateEmploymentStatus(this.employmentStatus.id, employmentStatusForm).subscribe(() => {
      this.alertify.success('Successfully updated');
    }, error => {
      this.alertify.error(error.error);
    });
  }

}
