import { Component, OnInit } from '@angular/core';
import { FormControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Observable, firstValueFrom, lastValueFrom } from 'rxjs';
import { Employee } from 'src/app/core/models/employee';
import { Group, InnerGroup, ModeratorGroups } from 'src/app/core/models/group';
import { Moderator, ModeratorMin } from 'src/app/core/models/moderator';
import { Organization } from 'src/app/core/models/organization';
import { FullTeam, InnerTeam, Team } from 'src/app/core/models/team';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EmployeeService } from 'src/app/core/services/employee.service';
import { GroupService } from 'src/app/core/services/group.service';
import { OrganizationService } from 'src/app/core/services/organization.service';
import { TeamService } from 'src/app/core/services/team.service';
import { faMinus, faPlus } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-employee-edit',
  templateUrl: './employee-edit.component.html'
})
export class EmployeeEditComponent implements OnInit {
  minusIcon = faMinus;
  plusIcon = faPlus;

  editEmployeeForm: FormGroup;
  employee: Employee;
  fullTeam: FullTeam;
  Moderator: Moderator;
  organizations$: Observable<Organization[]>;
  innerGroups: InnerGroup[];
  innerTeams: InnerTeam[];
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
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.editEmployeeForm = this.fb.group({
      firstName: this.firstName,
      lastName: this.lastName,
      organizationId: [null],
      groupId: [null],
      teamId: [null]
    });
    this.editEmployeeForm.get('organizationId').disable();

    this.route.data.subscribe(data => {
      this.employee = data['employee'];
      this.setupForm(this.employee);
    });

    if (this.employee.role === 'Moderator') {
      this.loadModeratorGroups(this.employee.teamId, this.employee.id);
    } else {
      this.loadFullTeam(this.employee.teamId);
    }

    this.organizations$ = this.organizationService.getOrganizations();
  }

  setupForm(employee: Employee): void {
    if (this.editEmployeeForm)
      this.editEmployeeForm.reset();

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
    this.loadInnerGroups(fullTeam.organizationId);
    this.editEmployeeForm.get('groupId').setValue(fullTeam.groupId);
    this.loadInnerTeams(fullTeam.groupId);
    this.editEmployeeForm.get('teamId').setValue(fullTeam.teamId);
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


  updateGroup(moderatorId: number, newGroupId: number): void {
    let moderatorGroups = {} as ModeratorGroups;
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
        this.loadFullTeam(employeeForm.teamId);
        this.alertify.success('Successfully updated');
      },
      error: () => {
        this.alertify.error('Unable to update employee');
      }
    });
  }
}
