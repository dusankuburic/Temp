import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { InnerGroups } from 'src/app/_models/group';

@Component({
  selector: 'app-group-list',
  templateUrl: './group-list.component.html',
  styleUrls: ['./group-list.component.scss']
})
export class GroupListComponent implements OnInit {

  innerGroups: InnerGroups;

  constructor(
    private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.innerGroups = data['innergroups'];
    });
  }

}
