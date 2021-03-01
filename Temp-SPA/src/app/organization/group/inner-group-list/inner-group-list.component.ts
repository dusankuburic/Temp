import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { InnerGroups } from 'src/app/_models/group';

@Component({
  selector: 'app-group-list',
  templateUrl: './inner-group-list.component.html',
  styleUrls: ['./inner-group-list.component.scss']
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
