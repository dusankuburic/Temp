import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faEdit, faPenNib, faProjectDiagram, faUsers } from '@fortawesome/free-solid-svg-icons';
import { InnerGroup, PagedInnerGroups } from 'src/app/core/models/group';
import { Organization } from 'src/app/core/models/organization';
import { Pagination } from 'src/app/core/models/pagination';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { GroupService } from 'src/app/core/services/group.service';

@Component({
  selector: 'app-group-list',
  templateUrl: './inner-group-list.component.html'
})
export class GroupListComponent implements OnInit {
  editGroupIcon = faEdit
  archiveGroupIcon = faPenNib
  innerTeamsIcon = faUsers
  createTeamIcon = faProjectDiagram

  innerGroups: InnerGroup[];
  pagination: Pagination;
  organization: Organization;

  constructor(
    private route: ActivatedRoute,
    private groupService: GroupService,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.organization = { id: data['innergroups'].id, name: data['innergroups'].name };
      this.innerGroups = data['innergroups'].groups.result;
      this.pagination = data['innergroups'].groups.pagination;
    });
  }

  loadGroups(): void {
    this.groupService.getInnerGroups(this.pagination.currentPage, this.pagination.itemsPerPage, this.organization.id)
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
    this.pagination.currentPage = event.page;
    this.loadGroups();
  }

  changeStatus(id: number): void {
    this.groupService.changeStatus(id).subscribe({
      next: () => {
        this.loadGroups();
        this.alertify.success('Status changed');
      },
      error: (error) => {
        this.alertify.error(error.error);
      }
    });
  }

}
