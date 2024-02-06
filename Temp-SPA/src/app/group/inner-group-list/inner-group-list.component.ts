import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { InnerGroups } from 'src/app/_models/group';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { GroupService } from 'src/app/_services/group.service';

@Component({
  selector: 'app-group-list',
  templateUrl: './inner-group-list.component.html'
})
export class GroupListComponent implements OnInit {

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

  changeStatus(id: number): any {
    this.groupService.changeStatus(id).subscribe(() => {
      this.loadGroups();
      this.alertify.success('Status changed');
    }, error => {
      this.alertify.error(error.error);
    })
  }

}
