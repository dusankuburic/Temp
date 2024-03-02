import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faEdit, faPenNib, faPlusCircle } from '@fortawesome/free-solid-svg-icons';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { InnerGroup } from 'src/app/core/models/group';
import { Pagination } from 'src/app/core/models/pagination';
import { InnerTeam, PagedInnerTeams, TeamParams } from 'src/app/core/models/team';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { TeamService } from 'src/app/core/services/team.service';

@Component({
  selector: 'app-team-list',
  templateUrl: './inner-team-list.component.html'
})
export class TeamListComponent implements OnInit {
  editTeamIcon = faEdit;
  archiveTeamIcon = faPenNib;
  plusIcon = faPlusCircle;

  filtersForm: FormGroup;
  innerTeams: InnerTeam[];
  pagination: Pagination;
  group: InnerGroup;
  teamParams: TeamParams;

  constructor(
    private route: ActivatedRoute,
    private teamService: TeamService,
    private alertify: AlertifyService,
    private fb: FormBuilder) { 
      this.teamParams = teamService.getTeamParams();

      this.filtersForm = this.fb.group({
        name: ['', Validators.minLength(1)]
      })

      const nameControl = this.filtersForm.get('name');
      nameControl.valueChanges.pipe(
        debounceTime(600),
        distinctUntilChanged()
      ).subscribe((searchFor) => {
        const params = this.teamService.getTeamParams();
        params.pageNumber = 1;
        params.name = searchFor;
        this.teamService.setTeamParams(params);
        this.teamParams = params;
        this.loadTeams();
      })
    }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.group = {id: data['innerteams'].id, name: data['innerteams'].name };
      this.innerTeams = data['innerteams'].teams.result;
      this.pagination = data['innerteams'].teams.pagination;
    });
  }

  loadTeams(): void {
    this.teamService.getInnerTeams(this.group.id)
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
    const params = this.teamService.getTeamParams();
    if (params.pageNumber !== event) {
      this.pagination.currentPage = event;
      params.pageNumber = event;
      this.teamService.setTeamParams(params);
      this.teamParams = params;
      this.loadTeams();
    }
  }

  changeStatus(id: number): void {
    this.teamService.changeStatus(id).subscribe({
      next: () => {
        this.loadTeams();
        this.alertify.success('Status changed');
      },
      error: () => {
        this.alertify.error('Unable to archive');
      }
    });
  }

}
