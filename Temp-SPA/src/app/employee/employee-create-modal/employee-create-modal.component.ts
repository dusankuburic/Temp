import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { takeUntil } from 'rxjs';
import { Employee } from 'src/app/core/models/employee';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EmployeeService } from 'src/app/core/services/employee.service';
import { GroupService } from 'src/app/core/services/group.service';
import { OrganizationService } from 'src/app/core/services/organization.service';
import { SelectionOption } from 'src/app/shared/components/tmp-select/tmp-select.component';
import { DestroyableComponent } from 'src/app/core/base/destroyable.component';
import { BlobDto, BlobResponse } from 'src/app/core/models/blob';

@Component({
    selector: 'app-employee-create-modal',
    templateUrl: './employee-create-modal.component.html',
    standalone: false
})
export class EmployeeCreateModalComponent extends DestroyableComponent implements OnInit{
  title?: string;
  createEmployeeForm!: FormGroup;
  employee!: Employee;
  organizationsSelect!: SelectionOption[];
  innerGroupsSelect!: SelectionOption[];
  innerTeamsSelect!: SelectionOption[];
  employeeFiles: BlobDto[] = [];
  profilePictureUrl?: string;

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
    public bsModalRef: BsModalRef) {
    super();
  }

  ngOnInit(): void {
    this.createEmployeeForm = this.fb.group({
      firstName: this.firstName,
      lastName: this.lastName,
      organizationId: [null, Validators.required],
      groupId: [null, Validators.required],
      teamId: [null, Validators.required]
    });
    
    this.organizationService.getOrganizationsForSelect()
      .pipe(takeUntil(this.destroy$))
      .subscribe(res => {
        this.organizationsSelect = [
          {value: null, display: 'Select Organization', hidden: true},
          ...res
        ];
    });
  }

  loadInnerGroups(id: number | null): void {
    if (id == null)
      return;
    this.organizationService.getInnerGroupsForSelect(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe((res) => {
        if (res !== null) {
          this.innerGroupsSelect = [
          {value: null, display: 'Select Group', hidden: true},
            ...res
          ];
          this.createEmployeeForm.get('groupId')?.setValue(null);
          this.innerTeamsSelect = [{value: null, display: 'Select Team', hidden: true}];
        }
      });
  }

  loadInnerTeams(id: number | null): void {
    if (id == null)
      return;
    this.groupService.getInnerTeamsForSelect(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe((res) => {
        if (res !== null) {
          this.innerTeamsSelect = [];
          this.innerTeamsSelect = [
            {value: null, display: 'Select Team', hidden: true},
            ...res
          ];
          this.createEmployeeForm.get('teamId')?.setValue(null);
        }
      });
  }

  create(): void {
    this.employee = {
      ...this.createEmployeeForm.value,
      profilePictureUrl: this.profilePictureUrl
    };
    this.employeeService.createEmployee(this.employee)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.bsModalRef.content.isSaved = true;
          this.alertify.success('Successfully created');
          this.createEmployeeForm.reset();
          this.employeeFiles = [];
          this.profilePictureUrl = undefined;
        },
        error: () => {
          this.alertify.error('Unable to create employee');
        }
      });
  }

  onFileUploaded(response: BlobResponse): void {
    if (!response.error && response.blob) {
      // For profile picture (images), store the URL
      if (response.blob.fileType === 'Image') {
        this.profilePictureUrl = response.blob.name;
      }
      this.employeeFiles = [...this.employeeFiles, response.blob];
    }
  }

  onFileDeleted(path: string): void {
    // If deleted file is the profile picture, clear it
    if (path === this.profilePictureUrl) {
      this.profilePictureUrl = undefined;
    }
    this.employeeFiles = this.employeeFiles.filter(f => f.name !== path);
  }
}
