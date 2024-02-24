import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faEdit, faPenNib } from '@fortawesome/free-solid-svg-icons';
import { InnerTeams } from 'src/app/core/models/team';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { TeamService } from 'src/app/core/services/team.service';

@Component({
  selector: 'app-team-list',
  templateUrl: './inner-team-list.component.html'
})
export class TeamListComponent implements OnInit {
  editTeamIcon = faEdit
  archiveTeamIcon = faPenNib

  innerTeams: InnerTeams;

  constructor(
    private route: ActivatedRoute,
    private teamService: TeamService,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.innerTeams = data['innerteams'];
    });
  }

  changeStatus(id: number): void {
    this.teamService.changeStatus(id).subscribe({
      next: () => {
        this.alertify.success('Status changed');
      },
      error: (error) => {
        this.alertify.error(error.error);
      }
    });
  }

}
