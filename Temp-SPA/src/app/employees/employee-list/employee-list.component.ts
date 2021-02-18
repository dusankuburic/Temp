import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Employee } from 'src/app/_models/employee';
import { UnassignRoleDto } from 'src/app/_models/unassignRoleDto';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { EmployeeService } from 'src/app/_services/employee.service';

@Component({
  selector: 'app-employee-list',
  templateUrl: './employee-list.component.html',
  styleUrls: ['./employee-list.component.scss']
})
export class EmployeeListComponent implements OnInit {
  employees: Employee[];
  unassignRoleDto = {} as UnassignRoleDto;

  constructor(
    private route: ActivatedRoute,
    private employeeService: EmployeeService,
    private router: Router,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.employees = data['employees'];
    });
  }

  loadEmployees(): void {
    this.employeeService.getEmployees().subscribe((res: Employee[]) => {
      this.employees = res;
    }, error => {
      this.alertify.error('Problem retriving data');
    })
  }

  removeRole(id: number): any {
    this.unassignRoleDto.id = id;
    this.employeeService.unassignRole(this.unassignRoleDto).subscribe(() => {
      this.loadEmployees();
      this.alertify.success('Removed role');
    }, error => {
      this.alertify.error(error.error);
    });
  }


}
