import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { Employee } from 'src/app/core/models/employee';
import { InnerGroup } from 'src/app/core/models/group';
import { Organization } from 'src/app/core/models/organization';
import { InnerTeam, Team } from 'src/app/core/models/team';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EmployeeService } from 'src/app/core/services/employee.service';
import { GroupService } from 'src/app/core/services/group.service';
import { OrganizationService } from 'src/app/core/services/organization.service';

@Component({
  selector: 'app-employee-create',
  templateUrl: './employee-create.component.html'
})
export class EmployeeCreateComponent implements OnInit {
  createEmployeeForm: UntypedFormGroup;
  employee: Employee;
  organizations: Organization[];
  innerGroups: InnerGroup[];
  innerTeams: InnerTeam[];

  constructor(
    private employeeService: EmployeeService,
    private organizationService: OrganizationService, 
    private groupService: GroupService,
    private alertify: AlertifyService,
    private fb: UntypedFormBuilder) { }

  ngOnInit(): void {
   this.organizationService.getOrganizations().subscribe((res) => {
    this.organizations = res;
   })
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
    if (id == null)
      return;
    this.innerTeams = [];
    this.organizationService.getInnerGroups(id).subscribe((res) => {
      if (res !== null) {
        this.innerGroups = res;
      } else {
        this.innerGroups = [];
        this.innerTeams = [];
      }
    });
  }

  loadInnerTeams(id): void {
    if (id == null)
      return;
    this.groupService.getInnerTeams(id).subscribe((res) => {
      if (res !== null) {
        this.innerTeams = res;
      } else {
       this.innerTeams = [];
      }
    });
  }

  create(): void {
    this.employee = { ...this.createEmployeeForm.value };
    this.employeeService.createEmployee(this.employee).subscribe({
      next: () => {
        this.alertify.success('Successfully created');
        this.createEmployeeForm.reset();
      },
      error: () => {
        this.alertify.error('Unable to create employee');
      }
    });
  }

}
