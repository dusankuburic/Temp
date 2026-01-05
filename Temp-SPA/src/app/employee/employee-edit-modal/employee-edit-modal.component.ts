import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { faFile, faMinus, faPlus } from '@fortawesome/free-solid-svg-icons';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { firstValueFrom, lastValueFrom } from 'rxjs';
import { takeUntil } from 'rxjs';
import { Employee } from 'src/app/core/models/employee';
import { Group, ModeratorGroups } from 'src/app/core/models/group';
import { ModeratorMin } from 'src/app/core/models/moderator';
import { FullTeam } from 'src/app/core/models/team';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EmployeeService } from 'src/app/core/services/employee.service';
import { GroupService } from 'src/app/core/services/group.service';
import { OrganizationService } from 'src/app/core/services/organization.service';
import { TeamService } from 'src/app/core/services/team.service';
import { SelectionOption } from 'src/app/shared/components/tmp-select/tmp-select.component';
import { DestroyableComponent } from 'src/app/core/base/destroyable.component';
import { BlobDto, BlobResponse } from 'src/app/core/models/blob';

@Component({
    selector: 'app-employee-edit-modal',
    templateUrl: './employee-edit-modal.component.html',
    styleUrls: ['../../shared/styles/modal.css'],
    standalone: false
})
export class EmployeeEditModalComponent extends DestroyableComponent implements OnInit { 
  minusIcon = faMinus;
  plusIcon = faPlus;
  fileIcon = faFile;

  title?: string;
  employeeId!: number;
  editEmployeeForm!: FormGroup;
  employee!: Employee;
  username?: string;
  fullTeam!: FullTeam;
  organizationsSelect!: SelectionOption[];
  innerGroupsSelect!: SelectionOption[];
  innerTeamsSelect!: SelectionOption[];
  currentModeratorGroups!: Group[];
  freeModeratorGroups!: Group[];
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
    private teamService: TeamService,
    private fb: FormBuilder,
    private alertify: AlertifyService,
    public bsModalRef: BsModalRef) {
    super();
  }

  ngOnInit(): void {
    this.editEmployeeForm = this.fb.group({
      firstName: this.firstName,
      lastName: this.lastName,
      organizationId: [null, Validators.required],
      groupId: [null, Validators.required],
      teamId: [null, Validators.required]
    });
    this.editEmployeeForm.get('organizationId')?.disable();

    this.employeeService.getEmployee(this.employeeId).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res: Employee) => {
        this.employee = res;
        this.setupForm(this.employee);

        if (this.employee.role !== 'None') {
          this.loadEmployeeUsername(this.employee.id);
        }

        if (this.employee.role === 'Moderator') {
          this.loadModeratorGroups(this.employee.teamId, this.employee.id, true);
        } else {
          this.loadFullTeam(this.employee.teamId);
        }

      },
      error: () => {
        this.alertify.error('Unable to get employee');
      }
    });

    this.organizationService.getOrganizationsForSelect()
      .pipe(takeUntil(this.destroy$)).subscribe(res => {
        this.organizationsSelect = [
          {value: null, display: '', hidden: true},
          ...res
        ];
      });
  }

  setupForm(employee: Employee): void {
      this.editEmployeeForm.patchValue({
        firstName: employee.firstName,
        lastName: employee.lastName
      });
      this.profilePictureUrl = employee.profilePictureUrl;
  }
  
  loadEmployeeUsername(employeeId: number) {
    this.employeeService.getEmployeeUsername(employeeId).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res: any) => {
        this.username = res.username;
      },
      error: () => {
        this.alertify.error('Unable to get employee username');
      }
    })
  }

  loadFullTeam(id: number): void {
    this.teamService.getFullTeam(id).pipe(takeUntil(this.destroy$)).subscribe((res) => {
      this.fullTeam = res;
      this.loadOrgData(this.fullTeam);
    });
  }

  async loadModeratorGroups(id: number, EmployeeId: number, isOnInit?: boolean) {
    try {
      const fullTeam = await lastValueFrom(this.teamService.getFullTeam(id));
      this.fullTeam = fullTeam;
      if (isOnInit)
        this.loadOrgData(this.fullTeam);

      const moderator = await firstValueFrom(this.employeeService.getEmployee(EmployeeId));
      const moderatorMin: ModeratorMin = {id: moderator.id};

      try {
        const currModerGroup = await firstValueFrom(this.groupService.getModeratorGroups(moderator.id));
        this.currentModeratorGroups = currModerGroup;
      } catch (error: any) {
        this.currentModeratorGroups = [];
        this.alertify.error(error.error);
      }

      try {
        const res = await firstValueFrom(this.groupService.getModeratorFreeGroups(this.fullTeam.organizationId, moderatorMin));
        this.freeModeratorGroups = res;
      } catch (error: any) {
        this.alertify.error(error.error);
      }
    } catch (error) {
      this.alertify.error('Failed to load moderator groups');
    }
  }

  loadOrgData(fullTeam: FullTeam) {
    this.editEmployeeForm.get('organizationId')?.setValue(fullTeam.organizationId);
    this.organizationService.getInnerGroupsForSelect(fullTeam.organizationId).pipe(takeUntil(this.destroy$)).subscribe((res) => {
      if (res !== null) {
        this.innerGroupsSelect = [
          {value: null, display: '', hidden: true},
          ...res
        ];
      }
    });

    this.editEmployeeForm.get('groupId')?.setValue(fullTeam.groupId);
    this.groupService.getInnerTeamsForSelect(fullTeam.groupId).pipe(takeUntil(this.destroy$)).subscribe((res) => {
      if (res !== null) {
        this.innerTeamsSelect = [];
        this.innerTeamsSelect = [
          {value: null, display: '', hidden: true},
          ...res
        ];
      }
    });
    this.editEmployeeForm.get('teamId')?.setValue(fullTeam.teamId);
  }

  loadInnerGroups(id: number | null): void {
    if (id == null)
      return;
    this.organizationService.getInnerGroupsForSelect(id).pipe(takeUntil(this.destroy$)).subscribe((res) => {
      if (res !== null) {
        this.innerGroupsSelect = [
          {value: null, display: '', hidden: true},
          ...res
        ];
        this.editEmployeeForm.get('teamId')?.setValue(null);
      }
    });
  }

  loadInnerTeams(id: number | null): void {
    if (id == null)
      return;
    this.groupService.getInnerTeamsForSelect(id).pipe(takeUntil(this.destroy$)).subscribe((res) => {
      if (res !== null) {
        this.innerTeamsSelect = [];
        this.innerTeamsSelect = [
          {value: null, display: '', hidden: true},
          ...res
        ];
        this.editEmployeeForm.get('teamId')?.setValue(null);
      }
    });
  }

  updateGroup(moderatorId: number, newGroupId: number): void {
    const moderatorGroups = {} as ModeratorGroups;
    moderatorGroups.groups = [];

    this.currentModeratorGroups.forEach((elem) => {
      moderatorGroups.groups.push(elem.id);
    });

    if (!moderatorGroups.groups.includes(newGroupId)) {
      moderatorGroups.groups.push(newGroupId);
    } else {
      moderatorGroups.groups = moderatorGroups.groups
      .filter(elem => elem !== newGroupId);
    }

    this.groupService.updateModeratorGroups(moderatorId, moderatorGroups.groups).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.loadModeratorGroups(this.employee.teamId, this.employee.id);
        this.alertify.success('Success');
      },
      error: () => {
        this.alertify.error('Unable to manage groups');
      }
    });
  }

  update(): void {
    const employeeForm = {
      ...this.editEmployeeForm.value,
      id: this.employee.id,
      profilePictureUrl: this.profilePictureUrl
    };

    if (employeeForm.teamId == null) {
      employeeForm.teamId = this.employee.teamId;
    }

    this.employeeService.updateEmployee(employeeForm).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.bsModalRef.content.isSaved = true;
        this.alertify.success('Successfully updated');
      },
      error: () => {
        this.alertify.error('Unable to update employee');
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
