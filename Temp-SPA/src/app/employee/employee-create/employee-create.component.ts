import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Employee } from 'src/app/_models/employee';
import { Group } from 'src/app/_models/group';
import { Organization } from 'src/app/_models/organization';
import { Team } from 'src/app/_models/team';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { EmployeeService } from 'src/app/_services/employee.service';
import { GroupService } from 'src/app/_services/group.service';
import { OrganizationService } from 'src/app/_services/organization.service';

@Component({
  selector: 'app-employee-create',
  templateUrl: './employee-create.component.html',
  styleUrls: ['./employee-create.component.scss']
})
export class EmployeeCreateComponent implements OnInit {
  createEmployeeForm: FormGroup;
  employee: Employee;
  organizations: Organization[];
  innerGroups = [] as Group[];
  innerTeams = [] as Team[];

  constructor(
    private route: ActivatedRoute,
    private employeeService: EmployeeService,
    private organizationService: OrganizationService,
    private groupService: GroupService,
    private alertify: AlertifyService,
    private fb: FormBuilder) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.organizations = data['organizations'];
    });
    this.createForm();
  }

  createForm(): void {
    this.createEmployeeForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      organizationId: [null, Validators.required],
      groupId: [null, Validators.required],
      teamId: [null, Validators.required]
    });
  }

  loadInnerGroups(id): void {
    this.innerTeams = [] as Team[];
    this.organizationService.getInnerGroups(this.sliceStringId(id)).subscribe((res) => {
      if (res !== null)
      {
        this.innerGroups = res.groups;
      }
      else {
        this.innerGroups = [] as Group[];
        this.innerTeams = [] as Team[];
      }
    });
  }

  loadInnerTeams(id): void {
    this.groupService.getInnerTeams(this.sliceStringId(id)).subscribe((res) => {
      if (res !== null) {
        this.innerTeams = res.teams;
      }
      else {
        this.innerTeams = [] as Team[];
      }
    });
  }

  // input => 1: 4343, 3: 3, 2: 5
  // output => 4343, 3, 5

  sliceStringId(str): number{
    return str.slice(3, str.length);
  }

  create(): any {
    this.employee = Object.assign({}, this.createEmployeeForm.value);
    this.employeeService.createEmployee(this.employee).subscribe(() => {
     // this.router.navigate(['employee/create']);
      this.alertify.success('Successfully created');
      this.createEmployeeForm.reset();
    }, error => {
      this.alertify.error(error);
    });
  }

}
