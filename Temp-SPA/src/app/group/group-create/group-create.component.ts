import { Component, OnInit } from '@angular/core';
import { FormControl, UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Group } from 'src/app/core/models/group';
import { Organization } from 'src/app/core/models/organization';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { GroupService } from 'src/app/core/services/group.service';
import { GroupValidators } from '../group-validators';

@Component({
  selector: 'app-group-create',
  templateUrl: './group-create.component.html'
})

export class GroupCreateComponent implements OnInit {
  createGroupForm: UntypedFormGroup;
  organization: Organization;
  group: Group;

  name = new FormControl('', [
    Validators.required,
    Validators.minLength(3),
    Validators.maxLength(60)]);

  constructor(
    private groupService: GroupService,
    private route: ActivatedRoute,
    private alertify: AlertifyService,
    private fb: UntypedFormBuilder,
    private validators: GroupValidators) { }

  ngOnInit(): void {
    this.createGroupForm = this.fb.group({
      name: this.name
    });

    this.route.data.subscribe(data => {
      this.organization = data['organization'];
      this.setupForm(this.organization);
    });
  }

  setupForm(organization: Organization): void {
    this.name.setAsyncValidators(this.validators.validateNameNotTaken(organization.id))
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
