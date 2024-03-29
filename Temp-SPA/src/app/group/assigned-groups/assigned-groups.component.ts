import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Group } from '../../core/models/group';
import { faUsers } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-assigned-groups',
  templateUrl: './assigned-groups.component.html'
})
export class AssignedGroupsComponent implements OnInit {
  usersIcon = faUsers;
  groups: Group[];
  constructor(private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.groups = data['groups'];
    });
  }

}
