import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-engagement-create',
  templateUrl: './engagement-create.component.html',
  styleUrls: ['./engagement-create.component.scss']
})
export class EngagementCreateComponent implements OnInit {
  employeeData: any;
  constructor(private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.employeeData = data['employeeData'];
    });
  }

}
