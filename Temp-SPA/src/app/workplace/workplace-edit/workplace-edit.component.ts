import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Workplace } from 'src/app/_models/workplace';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { WorkplaceService } from 'src/app/_services/workplace.service';

@Component({
  selector: 'app-workplace-edit',
  templateUrl: './workplace-edit.component.html'
})
export class WorkplaceEditComponent implements OnInit {
  editWorkplaceForm: FormGroup;
  workplace: Workplace;

  constructor(
    private workplaceService: WorkplaceService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
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
    this.workplaceService.updateWorkplace(this.workplace.id, workplaceForm).subscribe(() => {
      this.alertify.success('Successfully updated');
    }, error => {
      this.alertify.error(error.error);
    });
  }

}
