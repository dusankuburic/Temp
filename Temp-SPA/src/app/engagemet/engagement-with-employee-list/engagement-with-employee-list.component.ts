import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Employee } from 'src/app/_models/employee';

@Component({
  selector: 'app-engagement-with-employee-list',
  templateUrl: './engagement-with-employee-list.component.html',
  styleUrls: ['./engagement-with-employee-list.component.scss']
})
export class EngagementWithEmployeeListComponent implements OnInit {
  employees: Employee[];

  constructor(private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.employees = data['employeesWith'];
    });
  }

}
