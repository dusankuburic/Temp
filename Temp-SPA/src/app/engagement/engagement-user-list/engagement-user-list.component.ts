import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UserEngagement } from 'src/app/_models/engagement';

@Component({
  selector: 'app-engagement-user-list',
  templateUrl: './engagement-user-list.component.html'
})
export class EngagementUserListComponent implements OnInit {
  engagements: UserEngagement[];

  constructor(
    private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
        this.engagements = data['engagements'];

    });
  }

}
