import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { Workplace } from 'src/app/core/models/workplace';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { WorkplaceService } from 'src/app/core/services/workplace.service';

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

  create(): void {
    this.workplace = { ...this.createWorkplaceForm.value };
    this.workplaceService.createWorkplace(this.workplace).subscribe({
      next: () => {
        this.alertify.success('Successfully created');
        this.createWorkplaceForm.reset();
      },
      error: (error) => {
        this.alertify.error(error.error);
      }
    });
  }

}
