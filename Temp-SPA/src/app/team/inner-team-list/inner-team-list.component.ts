import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { InnerTeams } from 'src/app/models/team';
import { AlertifyService } from 'src/app/services/alertify.service';
import { TeamService } from 'src/app/services/team.service';

@Component({
  selector: 'app-team-list',
  templateUrl: './inner-team-list.component.html'
})
export class TeamListComponent implements OnInit {
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

  changeStatus(id: number): any {
    this.teamService.changeStatus(id).subscribe(() => {
      this.alertify.success('Status changed');
    }, error => {
      this.alertify.error(error.error);
    })
  }

}
