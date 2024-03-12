import { AfterViewInit, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faEdit, faPlusCircle, faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { Subscription, debounceTime, distinctUntilChanged } from 'rxjs';
import { InnerGroup } from 'src/app/core/models/group';
import { Pagination } from 'src/app/core/models/pagination';
import { InnerTeam, PagedInnerTeams, TeamParams } from 'src/app/core/models/team';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { TeamService } from 'src/app/core/services/team.service';
import { TeamCreateModalComponent } from '../team-create-modal/team-create-modal.component';
import { TeamEditModalComponent } from '../team-edit-modal/team-edit-modal.component';

@Component({
  selector: 'app-team-list',
  templateUrl: './inner-team-list.component.html'
})
export class TeamListComponent implements OnInit, AfterViewInit {
  editTeamIcon = faEdit;
  archiveTeamIcon = faTrashAlt;
  plusIcon = faPlusCircle;

  bsModalRef?: BsModalRef;
  subscriptions: Subscription;
  filtersForm: FormGroup;
  innerTeams: InnerTeam[];
  pagination: Pagination;
  group: InnerGroup;
  teamParams: TeamParams;

  constructor(
    private route: ActivatedRoute,
    private teamService: TeamService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private bsModalService: BsModalService) { 
      this.teamParams = teamService.getTeamParams();

      this.filtersForm = this.fb.group({
        name: ['']
      })
    }

  ngAfterViewInit(): void {
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
      this.group = {
        id: data['innerteams'].id, 
        name: data['innerteams'].name,
        hasActiveTeam: data['innerteams'].hasActiveTeam
      };
      this.innerTeams = data['innerteams'].teams.result;
      this.pagination = data['innerteams'].teams.pagination;
    });
  }

  openCreateModal(groupId: number): void {
    const initialState: ModalOptions = {
      class: 'modal-dialog-centered',
      initialState: {
        title: 'Create Team',
        groupId: groupId
      }
    };
    this.subscriptions = new Subscription();
    this.bsModalRef = this.bsModalService.show(TeamCreateModalComponent, initialState);
    if (this.bsModalRef?.onHidden) {
      this.subscriptions.add(this.bsModalRef.onHidden.subscribe(() => {
        if (this.bsModalRef.content.isSaved)
          this.loadTeams();
        
        this.unsubscribe();
      }))
    }
  }

  openEditModal(teamId: number, groupId: number): void {
    const initialState: ModalOptions = {
      class: 'modal-dialog-centered',
      initialState: {
        title: 'Edit Team',
        teamId: teamId,
        groupId: groupId
      }
    };
    this.subscriptions = new Subscription();
    this.bsModalRef = this.bsModalService.show(TeamEditModalComponent, initialState);
    if (this.bsModalRef?.onHidden) {
      this.subscriptions.add(this.bsModalRef.onHidden.subscribe(() => {
        if (this.bsModalRef.content.isSaved)
          this.loadTeams();

        this.unsubscribe();
      }))
    }
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

  unsubscribe() {
    this.subscriptions.unsubscribe();
  }


}
