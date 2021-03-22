import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ModeratorListApplication } from 'src/app/_models/application';

@Component({
  selector: 'app-application-moderator-list',
  templateUrl: './application-moderator-list.component.html'
})
export class ApplicationModeratorListComponent implements OnInit {

  applications: ModeratorListApplication[];
  constructor(private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.applications = data['applications'];
    });
    console.log(typeof this.applications);
  }

}
