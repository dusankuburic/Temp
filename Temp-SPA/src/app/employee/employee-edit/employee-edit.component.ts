import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Employee } from 'src/app/_models/employee';
import { Group } from 'src/app/_models/group';
import { Organization } from 'src/app/_models/organization';
import { Team } from 'src/app/_models/team';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { EmployeeService } from 'src/app/_services/employee.service';
import { GroupService } from 'src/app/_services/group.service';
import { OrganizationService } from 'src/app/_services/organization.service';

@Component({
  selector: 'app-employee-edit',
  templateUrl: './employee-edit.component.html',
  styleUrls: ['./employee-edit.component.scss']
})
export class EmployeeEditComponent implements OnInit {
  editEmployeeForm: FormGroup;
  employee: Employee;
  organizations: Organization[];
  innerGroups = [] as Group[];
  innerTeams = [] as Team[];

  constructor(
    private employeeService: EmployeeService,
    private organizationService: OrganizationService,
    private groupService: GroupService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.employee = data['employee'];
      this.organizations = data['organizations'];
    });
    this.createForm();
  }

  createForm(): void {
    this.editEmployeeForm = this.fb.group({
      firstName: [this.employee.firstName, Validators.required],
      lastName: [this.employee.lastName, Validators.required],
      organizationId: [null],
      groupId: [null],
      teamId: [null]
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

  sliceStringId(str): number{
    return str.slice(3, str.length);
  }

  update(): any {
    const employeeForm = Object.assign({}, this.editEmployeeForm.value);
    if (employeeForm.teamId == null)
    {
      employeeForm.teamId = this.employee.teamId;
    }
    console.log(employeeForm);
    this.employeeService.updateEmployee(this.employee.id, employeeForm).subscribe(() => {
      this.alertify.success('Successfully updated');
    }, error => {
      this.alertify.error(error);
    });
  }
}
