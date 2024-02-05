import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { Workplace } from 'src/app/_models/workplace';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { WorkplaceService } from 'src/app/_services/workplace.service';

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
