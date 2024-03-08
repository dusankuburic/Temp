import { Component, OnInit } from '@angular/core';
import { FormControl, UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Workplace } from 'src/app/core/models/workplace';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { WorkplaceService } from 'src/app/core/services/workplace.service';
import { WorkplaceValidators } from '../workplace-validators';

@Component({
  selector: 'app-workplace-edit',
  templateUrl: './workplace-edit.component.html'
})
export class WorkplaceEditComponent implements OnInit {
  editWorkplaceForm: UntypedFormGroup;
  workplace: Workplace;

  name = new FormControl('', [
    Validators.required, 
    Validators.minLength(3), 
    Validators.maxLength(60)]);

  constructor(
    private workplaceService: WorkplaceService,
    private route: ActivatedRoute,
    private fb: UntypedFormBuilder,
    private alertify: AlertifyService,
    private validators: WorkplaceValidators) { }

  ngOnInit(): void {
    this.editWorkplaceForm = this.fb.group({
      name: this.name
    });

    this.route.data.subscribe(data => {
      this.workplace = data['workplace'];
      this.setupForm(this.workplace);
    });
  }

  setupForm(workplace: Workplace): void {
    if (this.editWorkplaceForm) 
      this.editWorkplaceForm.reset();
     
    this.name.addAsyncValidators(this.validators.validateNameNotTaken(workplace.name))

    this.editWorkplaceForm.patchValue({
      name: workplace.name
    });
  }

  update(): void {
    const workplaceForm = { ...this.editWorkplaceForm.value, id: this.workplace.id};
    this.workplaceService.updateWorkplace(workplaceForm).subscribe({
      next: () => {
        this.alertify.success('Successfully updated');
      },
      error: () => {
        this.alertify.error('Unable to edit workplace');
      }
    });
  }

}
