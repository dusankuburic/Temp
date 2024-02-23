import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faEdit, faPenNib, faProjectDiagram, faUsers } from '@fortawesome/free-solid-svg-icons';
import { InnerGroups } from 'src/app/core/models/group';
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

  innerGroups: InnerGroups;

  constructor(
    private route: ActivatedRoute,
    private groupService: GroupService,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.innerGroups = data['innergroups'];
    });
  }

  loadGroups(): void {

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
