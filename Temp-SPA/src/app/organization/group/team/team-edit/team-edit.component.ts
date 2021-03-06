import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Team } from 'src/app/_models/team';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { TeamService } from 'src/app/_services/team.service';

@Component({
  selector: 'app-team-edit',
  templateUrl: './team-edit.component.html'
})
export class TeamEditComponent implements OnInit {
  editTeamForm: FormGroup;
  team: Team;

  constructor(
    private teamService: TeamService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.team = data['team'];
    });
    this.createForm();
  }

  createForm(): void {
    this.editTeamForm = this.fb.group({
      Name: [this.team.name, Validators.required]
    });
  }

  update(): void {
    const teamForm = Object.assign({}, this.editTeamForm.value);
    this.team.name = teamForm.Name;
    this.teamService.updateTeam(this.team.id, this.team).subscribe(() => {
      this.alertify.success('Successfully updated');
    }, error => {
      this.alertify.error(error.error);
    });
  }

}
