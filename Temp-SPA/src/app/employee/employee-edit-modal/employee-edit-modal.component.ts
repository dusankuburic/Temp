import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faMinus, faPlus } from '@fortawesome/free-solid-svg-icons';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { firstValueFrom, lastValueFrom } from 'rxjs';
import { Employee } from 'src/app/core/models/employee';
import { Group, ModeratorGroups } from 'src/app/core/models/group';
import { Moderator, ModeratorMin } from 'src/app/core/models/moderator';
import { FullTeam } from 'src/app/core/models/team';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EmployeeService } from 'src/app/core/services/employee.service';
import { GroupService } from 'src/app/core/services/group.service';
import { OrganizationService } from 'src/app/core/services/organization.service';
import { TeamService } from 'src/app/core/services/team.service';
import { SelectionOption } from 'src/app/shared/components/tmp-select/tmp-select.component';

@Component({
  selector: 'app-employee-edit-modal',
  templateUrl: './employee-edit-modal.component.html',
})
export class EmployeeEditModalComponent implements OnInit { 
  minusIcon = faMinus;
  plusIcon = faPlus;

  title?: string;
  employeeId: number;
  editEmployeeForm: FormGroup;
  employee: Employee;
  fullTeam: FullTeam;
  Moderator: Moderator;
  organizationsSelect: SelectionOption[];
  innerGroupsSelect: SelectionOption[];
  innerTeamsSelect: SelectionOption[];
  currentModeratorGroups: Group[];
  freeModeratorGroups: Group[];

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
    public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
    this.editEmployeeForm = this.fb.group({
      firstName: this.firstName,
      lastName: this.lastName,
      organizationId: [null, Validators.required],
      groupId: [null, Validators.required],
      teamId: [null, Validators.required]
    });
    this.editEmployeeForm.get('organizationId').disable();

    this.employeeService.getEmployee(this.employeeId).subscribe({
      next: (res: Employee) => {
        this.employee = res;
        this.setupForm(this.employee);
        
        if (this.employee.role === 'Moderator') {
          this.loadModeratorGroups(this.employee.teamId, this.employee.id);
        } else {
          this.loadFullTeam(this.employee.teamId);
        }

      },
      error: () => {
        this.alertify.error('Unable to get employee');
      }
    });

    this.organizationService.getOrganizationsForSelect()
      .subscribe(res => {
        this.organizationsSelect = [
          {value: null, display: 'Select Organization', hidden: true},
          ...res
        ];
      });
  }

  setupForm(employee: Employee): void {
      this.editEmployeeForm.patchValue({
        firstName: employee.firstName,
        lastName: employee.lastName
      });
  }

  loadFullTeam(id): void {
    this.teamService.getFullTeam(id).subscribe((res) => {
      this.fullTeam = res;
      this.loadOrgData(this.fullTeam);
    });
  }

  async loadModeratorGroups(id, EmployeeId: number) {
    let moderatorMin: ModeratorMin; 

    await lastValueFrom(this.teamService.getFullTeam(id)).then((fullTeam) => {
      this.fullTeam = fullTeam;
      this.loadOrgData(this.fullTeam);

      firstValueFrom(this.employeeService.getModerator(EmployeeId)).then((moderator) => {
        this.Moderator = moderator;
        moderatorMin = {id: moderator.id};

        firstValueFrom(this.groupService.getModeratorGroups(moderator.id)).then((currModerGroup) => {
          this.currentModeratorGroups = currModerGroup;
        }, error => {
          this.currentModeratorGroups = [];
          this.alertify.error(error.error);
        })

        firstValueFrom(this.groupService.getModeratorFreeGroups(this.fullTeam.organizationId, moderatorMin)).then((res) => {
          this.freeModeratorGroups = res;
        }, error => {
          this.alertify.error(error.error);
        });

      });
    });
  }

  loadOrgData(fullTeam: FullTeam) {
    this.editEmployeeForm.get('organizationId').setValue(fullTeam.organizationId);
    this.organizationService.getInnerGroupsForSelect(fullTeam.organizationId).subscribe((res) => {
      if (res !== null) {
        this.innerGroupsSelect = [
          {value: null, display: 'Select Group', hidden: true},
          ...res
        ];
      }
    });

    this.editEmployeeForm.get('groupId').setValue(fullTeam.groupId);
    this.groupService.getInnerTeamsForSelect(fullTeam.groupId).subscribe((res) => {
      if (res !== null) {
        this.innerTeamsSelect = [];
        this.innerTeamsSelect = [
          {value: null, display: 'Select Team', hidden: true},
          ...res
        ];
      }
    });
    this.editEmployeeForm.get('teamId').setValue(fullTeam.teamId);
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
        this.editEmployeeForm.get('teamId').setValue(null);
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
        this.editEmployeeForm.get('teamId').setValue(null);
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

    this.groupService.updateModeratorGroups(moderatorId, moderatorGroups).subscribe({
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
    const employeeForm = { ...this.editEmployeeForm.value, id: this.employee.id };

    if (employeeForm.teamId == null) {
      employeeForm.teamId = this.employee.teamId;
    }

    this.employeeService.updateEmployee(employeeForm).subscribe({
      next: () => {
        this.bsModalRef.content.isSaved = true;
        this.alertify.success('Successfully updated');
      },
      error: () => {
        this.alertify.error('Unable to update employee');
      }
    });
  }
}
