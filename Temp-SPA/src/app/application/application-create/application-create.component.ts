import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { CreateApplication } from 'src/app/_models/application';
import { Team } from 'src/app/_models/team';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ApplicationService } from 'src/app/_services/application.service';

@Component({
  selector: 'app-application-create',
  templateUrl: './application-create.component.html'
})
export class ApplicationCreateComponent implements OnInit {
  createApplicationForm: FormGroup;
  application: CreateApplication;
  team: Team;
  user: any;

  constructor(
    private applicationService: ApplicationService,
    private route: ActivatedRoute,
    private alertify: AlertifyService,
    private fb: FormBuilder) { }

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

  create(): any {
    this.application = Object.assign({}, this.createApplicationForm.value);
    this.application.teamId = this.team.id;
    this.application.userId = this.user.id;
    this.applicationService.createApplication(this.application).subscribe(() => {
      this.alertify.success('Successfully created');
      this.createApplicationForm.reset();
    }, error => {
      this.alertify.error(error);
    });

  }

}
