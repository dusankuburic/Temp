import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Workplace } from 'src/app/models/workplace';
import { AlertifyService } from 'src/app/services/alertify.service';
import { WorkplaceService } from 'src/app/services/workplace.service';

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

  createForm(): void {
    this.editWorkplaceForm = this.fb.group({
      name: [this.workplace.name, Validators.required]
    });
  }

  update(): any {
    const workplaceForm = Object.assign({}, this.editWorkplaceForm.value);
    workplaceForm.id = this.workplace.id;
    this.workplaceService.updateWorkplace(workplaceForm).subscribe(() => {
      this.alertify.success('Successfully updated');
    }, error => {
      this.alertify.error(error.error);
    });
  }

}
