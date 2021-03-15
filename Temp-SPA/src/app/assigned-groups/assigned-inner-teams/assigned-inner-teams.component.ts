import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { InnerTeams } from 'src/app/_models/team';

@Component({
  selector: 'app-assigned-inner-teams',
  templateUrl: './assigned-inner-teams.component.html'
})
export class AssignedInnerTeamsComponent implements OnInit {

  innerTeams: InnerTeams;

  constructor(
    private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.innerTeams = data['teams'];
    });
  }

}
