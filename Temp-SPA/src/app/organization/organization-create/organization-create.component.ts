import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { Organization } from 'src/app/_models/organization';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { OrganizationService } from 'src/app/_services/organization.service';

@Component({
  selector: 'app-organization-create',
  templateUrl: './organization-create.component.html'
})
export class OrganizationCreateComponent implements OnInit {
  createOrganizationForm: UntypedFormGroup;
  organization: Organization;

  constructor(
    private organizationService: OrganizationService,
    private alertify: AlertifyService,
    private fb: UntypedFormBuilder) { }

  ngOnInit(): void {
    this.createForm();
  }

  createForm(): void {
    this.createOrganizationForm = this.fb.group({
      Name: ['', Validators.required]
    });
  }

  create(): any {
    this.organization = Object.assign({}, this.createOrganizationForm.value);
    this.organizationService.createOrganization(this.organization).subscribe(() => {
      this.alertify.success('Successfully created');
      this.createOrganizationForm.reset();
    }, error => {
      this.alertify.error(error.error);
    });
  }
}
