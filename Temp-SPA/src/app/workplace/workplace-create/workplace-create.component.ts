import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { Workplace } from 'src/app/models/workplace';
import { AlertifyService } from 'src/app/services/alertify.service';
import { WorkplaceService } from 'src/app/services/workplace.service';

@Component({
  selector: 'app-workplace-create',
  templateUrl: './workplace-create.component.html'
})
export class WorkplaceCreateComponent implements OnInit {
  createWorkplaceForm: UntypedFormGroup;
  workplace: Workplace;

  constructor(
    private workplaceService: WorkplaceService,
    private alertify: AlertifyService,
    private fb: UntypedFormBuilder) { }

  ngOnInit(): void {
    this.createForm();
  }

  createForm(): void {
    this.createWorkplaceForm = this.fb.group({
      name: ['', Validators.required]
    });
  }

  create(): any {
    this.workplace = Object.assign({}, this.createWorkplaceForm.value);
    this.workplaceService.createWorkplace(this.workplace).subscribe(() => {
      this.alertify.success('Successfully created');
      this.createWorkplaceForm.reset();
    }, error => {
      this.alertify.error(error.error);
    });
  }

}
