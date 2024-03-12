import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Employee } from 'src/app/core/models/employee';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EmployeeService } from 'src/app/core/services/employee.service';
import { GroupService } from 'src/app/core/services/group.service';
import { OrganizationService } from 'src/app/core/services/organization.service';
import { SelectionOption } from 'src/app/shared/components/tmp-select/tmp-select.component';

@Component({
  selector: 'app-employee-create-modal',
  templateUrl: './employee-create-modal.component.html',
})
export class EmployeeCreateModalComponent implements OnInit{
  title?: string;
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
    private fb: FormBuilder,
    public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
    this.createEmployeeForm = this.fb.group({
      firstName: this.firstName,
      lastName: this.lastName,
      organizationId: [null, Validators.required],
      groupId: [null, Validators.required],
      teamId: [null, Validators.required]
    });
    
    this.organizationService.getOrganizationsForSelect()
      .subscribe(res => {
        this.organizationsSelect = [
          {value: null, display: 'Select Organization', hidden: true},
          ...res
        ];
    });
  }

  loadInnerGroups(id): void {
    if (id == null)
      return;
    this.organizationService.getInnerGroupsForSelect(id).subscribe((res) => {
      if (res !== null) {
        this.innerGroupsSelect = [
        {value: null, display: 'Select Group', hidden: true},
          ...res
        ];
        this.createEmployeeForm.get('groupId').setValue(null);
        this.innerTeamsSelect = [{value: null, display: 'Select Team', hidden: true}];
      }
    });
  }

  loadInnerTeams(id): void {
    if (id == null)
      return;
    this.groupService.getInnerTeamsForSelect(id).subscribe((res) => {
      if (res !== null) {
        this.innerTeamsSelect = [];
        this.innerTeamsSelect = [
          {value: null, display: 'Select Team', hidden: true},
          ...res
        ];
        this.createEmployeeForm.get('teamId').setValue(null);
      }
    });
  }

  create(): void {
    this.employee = { ...this.createEmployeeForm.value };
    this.employeeService.createEmployee(this.employee).subscribe({
      next: () => {
        this.bsModalRef.content.isSaved = true;
        this.alertify.success('Successfully created');
        this.createEmployeeForm.reset();
      },
      error: () => {
        this.alertify.error('Unable to create employee');
      }
    });
  }
}
