import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { EmploymentStatus } from 'src/app/_models/employmentStatus';

@Component({
  selector: 'app-employment-status-list',
  templateUrl: './employment-status-list.component.html',
  styleUrls: ['./employment-status-list.component.scss']
})
export class EmploymentStatusListComponent implements OnInit {
  employmentStatuses: EmploymentStatus[];

  constructor(private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.employmentStatuses = data['employmentStatuses'];
    });
  }

}
