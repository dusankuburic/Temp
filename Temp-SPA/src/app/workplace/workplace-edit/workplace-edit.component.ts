import { Component, OnInit } from '@angular/core';
import { FormControl, UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Workplace } from 'src/app/core/models/workplace';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { WorkplaceService } from 'src/app/core/services/workplace.service';

@Component({
  selector: 'app-workplace-edit',
  templateUrl: './workplace-edit.component.html'
})
export class WorkplaceEditComponent implements OnInit {
  editWorkplaceForm: UntypedFormGroup;
  workplace: Workplace;

  constructor(
    private workplaceService: WorkplaceService,
    private route: ActivatedRoute,
    private fb: UntypedFormBuilder,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.workplace = data['workplace'];
    });
    this.createForm();
  }

  name = new FormControl('', [
    Validators.required, 
    Validators.minLength(3), 
    Validators.maxLength(60)])

  createForm(): void {
    this.editWorkplaceForm = this.fb.group({
      name: this.name.setValue(this.workplace.name)
    });
  }

  update(): void {
    const workplaceForm = { ...this.editWorkplaceForm.value, id: this.workplace.id};
    this.workplaceService.updateWorkplace(workplaceForm).subscribe({
      next: () => {
        this.alertify.success('Successfully updated');
      },
      error: (error) => {
        this.alertify.error(error.error);
      }
    });
  }

}
