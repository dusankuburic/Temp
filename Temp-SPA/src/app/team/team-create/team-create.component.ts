import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Group } from 'src/app/core/models/group';
import { Team } from 'src/app/core/models/team';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { TeamService } from 'src/app/core/services/team.service';

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

  create(): void {
    this.team = {...this.createTeamForm.value, groupId: this.group.id};
    this.teamService.createTeam(this.team).subscribe({
      next: () => {
        this.alertify.success('Successfully created')
        this.createTeamForm.reset();
      },
      error: (error) => {
        this.alertify.error(error.error);
      }
    });
  }

}
