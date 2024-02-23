import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Group } from 'src/app/core/models/group';
import { Organization } from 'src/app/core/models/organization';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { GroupService } from 'src/app/core/services/group.service';

@Component({
  selector: 'app-group-create',
  templateUrl: './group-create.component.html'
})

export class GroupCreateComponent implements OnInit {
  createGroupForm: UntypedFormGroup;
  organization: Organization;
  group: Group;

  constructor(
    private groupService: GroupService,
    private route: ActivatedRoute,
    private alertify: AlertifyService,
    private fb: UntypedFormBuilder) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.organization = data['organization'];
    });
    this.createForm();
  }

  createForm(): void {
    this.createGroupForm = this.fb.group({
      Name: ['', Validators.required]
    });
  }

  create(): void {
    this.group = {...this.createGroupForm.value, organizationId: this.organization.id };
    this.groupService.createGroup(this.group).subscribe({
      next: () => {
        this.alertify.success('Successfully created');
        this.createGroupForm.reset();
      },
      error: (error) => {
        this.alertify.error(error.error);
      }
    });
  
  }
}
