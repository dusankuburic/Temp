import { Component, OnInit } from '@angular/core';
import { FormControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Group } from 'src/app/core/models/group';
import { Team } from 'src/app/core/models/team';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { TeamService } from 'src/app/core/services/team.service';
import { TeamValidators } from '../team-validators';

@Component({
  selector: 'app-team-create',
  templateUrl: './team-create.component.html'
})
export class TeamCreateComponent implements OnInit {
  createTeamForm: FormGroup;
  group: Group;
  team: Team;
  
  name = new FormControl('',[
    Validators.required,
    Validators.minLength(3),
    Validators.maxLength(60)]);

  constructor(
    private teamService: TeamService,
    private route: ActivatedRoute,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private validators: TeamValidators) { }

  ngOnInit(): void {
    this.createTeamForm = this.fb.group({
      name: this.name
    });

    this.route.data.subscribe(data => {
      this.group = data['group'];
      this.setupForm(this.group);
    });
  }

  setupForm(group: Group): void {
    this.name.setAsyncValidators(this.validators.validateNameNotTaken(group.id))
  }

  create(): void {
    this.team = {...this.createTeamForm.value, groupId: this.group.id};
    this.teamService.createTeam(this.team).subscribe({
      next: () => {
        this.alertify.success('Successfully created')
        this.createTeamForm.reset();
      },
      error: () => {
        this.alertify.error('Unable to create team');
      }
    });
  }
}
