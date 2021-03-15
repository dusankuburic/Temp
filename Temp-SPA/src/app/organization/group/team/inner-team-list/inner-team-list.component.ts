import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { InnerTeams } from 'src/app/_models/team';

@Component({
  selector: 'app-team-list',
  templateUrl: './inner-team-list.component.html'
})
export class TeamListComponent implements OnInit {

  innerTeams: InnerTeams;

  constructor(
    private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.innerTeams = data['innerteams'];
    });
  }

}
