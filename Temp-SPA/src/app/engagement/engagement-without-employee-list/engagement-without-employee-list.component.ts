import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Employee } from 'src/app/_models/employee';

@Component({
  selector: 'app-engagement-without-employee-list',
  templateUrl: './engagement-without-employee-list.component.html',
  styleUrls: ['./engagement-without-employee-list.component.scss']
})
export class EngagementWithoutEmployeeListComponent implements OnInit {
  employees: Employee[];

  constructor(private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.employees = data['employeesWithout'];
    });
  }

}
