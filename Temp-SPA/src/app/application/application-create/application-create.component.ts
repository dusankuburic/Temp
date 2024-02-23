import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { CreateApplication } from 'src/app/core/models/application';
import { Team } from 'src/app/core/models/team';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { ApplicationService } from 'src/app/core/services/application.service';

@Component({
  selector: 'app-application-create',
  templateUrl: './application-create.component.html'
})
export class ApplicationCreateComponent implements OnInit {
  createApplicationForm: UntypedFormGroup;
  application: CreateApplication;
  team: Team;
  user: any;

  constructor(
    private applicationService: ApplicationService,
    private route: ActivatedRoute,
    private alertify: AlertifyService,
    private fb: UntypedFormBuilder) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.team = data['team'];
    });
    this.user = JSON.parse(localStorage.getItem('user'));
    this.createForm();
  }

  createForm(): void {
    this.createApplicationForm = this.fb.group({
      category: ['', Validators.required],
      content: ['', [ Validators.required, Validators.maxLength(600)]]
    });
  }

  create(): void {
    this.application = {...this.createApplicationForm.value, teamId: this.team.id, userId: this.user.id};
    this.applicationService.createApplication(this.application).subscribe({
      next: () => {
        this.alertify.success('Successfully created');
        this.createApplicationForm.reset();
      },
      error: (error) => {
        this.alertify.error(error);
      }
    });
  }
}
