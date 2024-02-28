import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faEdit, faPenNib } from '@fortawesome/free-solid-svg-icons';
import { InnerGroup } from 'src/app/core/models/group';
import { Pagination } from 'src/app/core/models/pagination';
import { InnerTeam, PagedInnerTeams } from 'src/app/core/models/team';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { TeamService } from 'src/app/core/services/team.service';

@Component({
  selector: 'app-team-list',
  templateUrl: './inner-team-list.component.html'
})
export class TeamListComponent implements OnInit {
  editTeamIcon = faEdit
  archiveTeamIcon = faPenNib

  innerTeams: InnerTeam[];
  pagination: Pagination;
  group: InnerGroup;

  constructor(
    private route: ActivatedRoute,
    private teamService: TeamService,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.group = {id: data['innerteams'].id, name: data['innerteams'].name };
      this.innerTeams = data['innerteams'].teams.result;
      this.pagination = data['innerteams'].teams.pagination;
    });
  }

  loadTeams(): void {
    this.teamService.getInnerTeams(this.pagination.currentPage, this.pagination.itemsPerPage, this.group.id)
      .subscribe({
        next: (res: PagedInnerTeams) => {
          this.innerTeams = res.teams.result;
          this.pagination = res.teams.pagination;
        },
        error: () => {
          this.alertify.error('Unable to load teams');
        }
      })
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadTeams();
  }

  changeStatus(id: number): void {
    this.teamService.changeStatus(id).subscribe({
      next: () => {
        this.loadTeams();
        this.alertify.success('Status changed');
      },
      error: (error) => {
        this.alertify.error(error.error);
      }
    });
  }

}
