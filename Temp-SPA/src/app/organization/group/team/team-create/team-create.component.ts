import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Group } from 'src/app/_models/group';
import { Team } from 'src/app/_models/team';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { TeamService } from 'src/app/_services/team.service';

@Component({
  selector: 'app-team-create',
  templateUrl: './team-create.component.html'
})
export class TeamCreateComponent implements OnInit {
  createTeamForm: UntypedFormGroup;
  group: Group;
  team: Team;

  constructor(
    private teamService: TeamService,
    private route: ActivatedRoute,
    private alertify: AlertifyService,
    private fb: UntypedFormBuilder) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.group = data['group'];
    });
    this.createForm();
  }

  createForm(): void {
    this.createTeamForm = this.fb.group({
      Name: ['', Validators.required]
    });
  }

  create(): any {
    this.team = Object.assign({}, this.createTeamForm.value);
    this.team.groupId = this.group.id;

    this.teamService.createTeam(this.team).subscribe(() => {
      this.alertify.success('Successfully created')
      this.createTeamForm.reset();
    }, error => {
      this.alertify.error(error.error);
    });
  }

}
