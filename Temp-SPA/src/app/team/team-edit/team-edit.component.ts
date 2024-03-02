import { Component, OnInit } from '@angular/core';
import { FormControl, UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Team } from 'src/app/core/models/team';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { TeamService } from 'src/app/core/services/team.service';

@Component({
  selector: 'app-team-edit',
  templateUrl: './team-edit.component.html'
})
export class TeamEditComponent implements OnInit {
  editTeamForm: UntypedFormGroup;
  team: Team;

  name = new FormControl('', [
    Validators.required,
    Validators.minLength(3),
    Validators.maxLength(60)
  ]);

  constructor(
    private teamService: TeamService,
    private route: ActivatedRoute,
    private fb: UntypedFormBuilder,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.editTeamForm = this.fb.group({
      name: this.name
    });

    this.route.data.subscribe(data => {
      this.team = data['team'];
      this.setupForm(this.team);
    });
  }

  setupForm(team: Team) {
    if (this.editTeamForm)
      this.editTeamForm.reset();

      this.editTeamForm.patchValue({
        name: team.name
      });
  }

  update(): void {
    const teamForm = { ...this.editTeamForm.value };
    this.team.name = teamForm.name;
    this.teamService.updateTeam(this.team.id, this.team).subscribe({
      next: () => {
        this.alertify.success('Successfully updated');
      },
      error: (error) => {
        this.alertify.error(error.error);
      }
    });
  }

}
