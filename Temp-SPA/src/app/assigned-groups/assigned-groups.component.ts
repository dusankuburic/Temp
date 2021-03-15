import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Group } from '../_models/group';

@Component({
  selector: 'app-assigned-groups',
  templateUrl: './assigned-groups.component.html'
})
export class AssignedGroupsComponent implements OnInit {
  groups: Group[];
  constructor(private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.groups = data['groups'];
    });
  }

}
