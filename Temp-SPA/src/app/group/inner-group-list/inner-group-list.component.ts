import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faEdit, faPlusCircle, faProjectDiagram, faTrashAlt, faUsers } from '@fortawesome/free-solid-svg-icons';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { GroupParams, InnerGroup, PagedInnerGroups } from 'src/app/core/models/group';
import { Organization } from 'src/app/core/models/organization';
import { Pagination } from 'src/app/core/models/pagination';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { GroupService } from 'src/app/core/services/group.service';
import { SelectionOption } from 'src/app/shared/components/tmp-select/tmp-select.component';

@Component({
  selector: 'app-group-list',
  templateUrl: './inner-group-list.component.html'
})
export class GroupListComponent implements OnInit {
  editGroupIcon = faEdit;
  archiveGroupIcon = faTrashAlt;
  innerTeamsIcon = faUsers;
  createTeamIcon = faProjectDiagram;
  plusIcon = faPlusCircle;

  filtersForm: FormGroup;
  innerGroups: InnerGroup[];
  pagination: Pagination;
  organization: Organization;
  groupParams: GroupParams;
  teamSelect: SelectionOption[] = [
    {value: '', display: 'Select with teams', disabled: true},
    {value: 'all', display: 'All'},
    {value: 'yes', display: 'With teams'},
    {value: 'no', display: 'Without teams'},
  ];

  constructor(
    private route: ActivatedRoute,
    private groupService: GroupService,
    private alertify: AlertifyService,
    private fb: FormBuilder) { 
      this.groupParams = groupService.getGroupParams();

      this.filtersForm = this.fb.group({
        withTeams: [''],
        name: ['']
      })

      const withTeamsControl = this.filtersForm.get('withTeams');
      withTeamsControl.valueChanges.pipe(
        debounceTime(100),
        distinctUntilChanged()
      ).subscribe((searchFor) => {
        const params = this.groupService.getGroupParams();
        params.pageNumber = 1;
        this.groupParams.withTeams = searchFor;
        this.groupService.setGroupParams(params);
        this.groupParams = params;
        this.loadGroups();
      });

      const nameControl = this.filtersForm.get('name');
      nameControl.valueChanges.pipe(
        debounceTime(600),
        distinctUntilChanged()
      ).subscribe((searchFor) => {
        const params = this.groupService.getGroupParams();
        params.pageNumber = 1;
        params.name = searchFor;
        this.groupService.setGroupParams(params);
        this.groupParams = params;
        this.loadGroups();
      })
    }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.organization = { 
        id: data['innergroups'].id, 
        name: data['innergroups'].name, 
        hasActiveGroup: data['innergroups'].hasActiveGroup 
      };
      this.innerGroups = data['innergroups'].groups.result;
      this.pagination = data['innergroups'].groups.pagination;
    });
  }

  loadGroups(): void {
    this.groupService.getInnerGroups(this.organization.id)
      .subscribe({
        next: (res: PagedInnerGroups) => {
          this.innerGroups = res.groups.result;
          this.pagination = res.groups.pagination;
        },
        error: () => {
          this.alertify.error('Unable to load groups');
        }
      })
  }

  pageChanged(event: any): void {
    const params = this.groupService.getGroupParams();
    if (params.pageNumber !== event) {
      this.pagination.currentPage = event;
      params.pageNumber = event;
      this.groupService.setGroupParams(params);
      this.groupParams = params;
      this.loadGroups();
    }
  }

  changeStatus(id: number): void {
    this.groupService.changeStatus(id).subscribe({
      next: () => {
        this.loadGroups();
        this.alertify.success('Status changed');
      },
      error: () => {
        this.alertify.error('Unable to archive');
      }
    });
  }

}
