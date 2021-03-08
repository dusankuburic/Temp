import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Employee } from 'src/app/_models/employee';
import { Group } from 'src/app/_models/group';
import { Moderator } from 'src/app/_models/moderator';
import { Organization } from 'src/app/_models/organization';
import { FullTeam, Team } from 'src/app/_models/team';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { EmployeeService } from 'src/app/_services/employee.service';
import { GroupService } from 'src/app/_services/group.service';
import { OrganizationService } from 'src/app/_services/organization.service';
import { TeamService } from 'src/app/_services/team.service';

@Component({
  selector: 'app-employee-edit',
  templateUrl: './employee-edit.component.html',
  styleUrls: ['./employee-edit.component.scss']
})
export class EmployeeEditComponent implements OnInit {
  editEmployeeForm: FormGroup;
  employee: Employee;
  fullTeam: FullTeam;
  Moderator: Moderator;
  organizations: Organization[];
  innerGroups = [] as Group[];
  innerTeams = [] as Team[];
  currentModeratorGroups = [] as Group[];
  freeModeratorGroups = [] as Group[];

  constructor(
    private employeeService: EmployeeService,
    private organizationService: OrganizationService,
    private groupService: GroupService,
    private teamService: TeamService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.employee = data['employee'];
      this.organizations = data['organizations'];
    });

    this.createForm();

    if (this.employee.role === 'Moderator')
    {
      this.loadModeratorGroups(this.employee.teamId, this.employee.id);
    }
    else 
    {
      this.loadFullTeam(this.employee.teamId);
    }

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

  loadFullTeam(id): void {
    this.teamService.getFullTeam(id).subscribe((res) => {
      this.fullTeam = res;
    });
  }

  loadModeratorGroups(id, EmployeeId: number): void {

    this.teamService.getFullTeam(id).toPromise().then((fullTeam) => {
      this.fullTeam = fullTeam;

      this.employeeService.getModerator(EmployeeId).toPromise().then((moderator) => {
        this.Moderator = moderator;

        this.groupService.getModeratorGroups(moderator.id).toPromise().then((currModerGroup) => {
          this.currentModeratorGroups = currModerGroup;
        });

        this.groupService.getModeratorFreeGroups(this.fullTeam.organizationId).toPromise().then((res)=> {
          this.freeModeratorGroups = res;
        });

      });
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
    this.employeeService.updateEmployee(this.employee.id, employeeForm).subscribe(() => {
      this.loadFullTeam(employeeForm.teamId);
      this.alertify.success('Successfully updated');
    }, error => {
      this.alertify.error(error);
    });
  }
}
