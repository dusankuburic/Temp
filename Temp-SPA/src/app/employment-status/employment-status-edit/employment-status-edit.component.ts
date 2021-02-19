import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { EmploymentStatus } from 'src/app/_models/employmentStatus';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { EmploymentStatusService } from 'src/app/_services/employment-status.service';

@Component({
  selector: 'app-employment-status-edit',
  templateUrl: './employment-status-edit.component.html',
  styleUrls: ['./employment-status-edit.component.scss']
})
export class EmploymentStatusEditComponent implements OnInit {
  editEmploymentStatusForm: FormGroup;
  employmentStatus: EmploymentStatus;

  constructor(
    private employmentStatusService: EmploymentStatusService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
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
