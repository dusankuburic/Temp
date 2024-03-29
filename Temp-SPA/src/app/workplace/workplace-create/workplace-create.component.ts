import { Component, OnInit } from '@angular/core';
import { FormControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Workplace } from 'src/app/core/models/workplace';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { WorkplaceService } from 'src/app/core/services/workplace.service';
import { WorkplaceValidators } from '../workplace-validators';

@Component({
  selector: 'app-workplace-create',
  templateUrl: './workplace-create.component.html'
})
export class WorkplaceCreateComponent implements OnInit {
  createWorkplaceForm: FormGroup;
  workplace: Workplace;

  name = new FormControl('', [
    Validators.required, 
    Validators.minLength(3), 
    Validators.maxLength(60)], 
    [this.validators.validateNameNotTaken()]);

  constructor(
    private workplaceService: WorkplaceService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private validators: WorkplaceValidators) { }

  ngOnInit(): void {
    this.createWorkplaceForm = this.fb.group({
      name: this.name
    });
  }

  create(): void {
    this.workplace = { ...this.createWorkplaceForm.value };
    this.workplaceService.createWorkplace(this.workplace).subscribe({
      next: () => {
        this.alertify.success('Successfully created');
        this.createWorkplaceForm.reset();
      },
      error: () => {
        this.alertify.error('Unable to create workplace');
      }
    });
  }

}
