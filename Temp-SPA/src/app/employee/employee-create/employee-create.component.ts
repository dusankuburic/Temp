import { Component, OnInit } from '@angular/core';
import { FormControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Employee } from 'src/app/core/models/employee';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EmployeeService } from 'src/app/core/services/employee.service';
import { GroupService } from 'src/app/core/services/group.service';
import { OrganizationService } from 'src/app/core/services/organization.service';
import { SelectionOption } from 'src/app/shared/components/tmp-select/tmp-select.component';

@Component({
  selector: 'app-employee-create',
  templateUrl: './employee-create.component.html'
})
export class EmployeeCreateComponent implements OnInit {
  createEmployeeForm: FormGroup;
  employee: Employee;
  organizationsSelect: SelectionOption[];
  innerGroupsSelect: SelectionOption[];
  innerTeamsSelect: SelectionOption[];

  firstName = new FormControl('', [
    Validators.required,
    Validators.minLength(3),
    Validators.maxLength(60)
  ]);

  lastName = new FormControl('', [
    Validators.required,
    Validators.minLength(3),
    Validators.maxLength(60)
  ]);

  constructor(
    private employeeService: EmployeeService,
    private organizationService: OrganizationService, 
    private groupService: GroupService,
    private alertify: AlertifyService,
    private fb: FormBuilder) { }

  ngOnInit(): void {
    this.createForm();
    this.organizationService.getOrganizationsForSelect()
      .subscribe(res => {
        this.organizationsSelect = [
          {value: null, display: 'Select Organization', hidden: true},
          ...res
        ];
    });
  }

  createForm(): void {
    this.createEmployeeForm = this.fb.group({
      firstName: this.firstName,
      lastName: this.lastName,
      organizationId: [null, Validators.required],
      groupId: [null, Validators.required],
      teamId: [null, Validators.required]
    });
  }

  loadInnerGroups(id): void {
    if (id == null)
      return;
    this.innerTeamsSelect = [];
    this.organizationService.getInnerGroupsForSelect(id).subscribe((res) => {
      if (res !== null) {
        this.innerGroupsSelect = [
        {value: null, display: 'Select Group', hidden: true},
          ...res
        ];
      } else {
        this.innerGroupsSelect = [];
        this.innerTeamsSelect = [];
      }
    });
  }

  loadInnerTeams(id): void {
    if (id == null)
      return;
    this.groupService.getInnerTeamsForSelect(id).subscribe((res) => {
      if (res !== null) {
        this.innerTeamsSelect = [
          {value: null, display: 'Select Team', hidden: true},
          ...res
        ];
      } else {
       this.innerTeamsSelect = [];
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
