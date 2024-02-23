import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { Organization } from 'src/app/core/models/organization';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { OrganizationService } from 'src/app/core/services/organization.service';

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

  create(): void {
    this.organization = {...this.createOrganizationForm.value};
    this.organizationService.createOrganization(this.organization).subscribe({
      next: () => {
        this.alertify.success('Successfully created');
        this.createOrganizationForm.reset();
      },
      error: (error) => {
        this.alertify.error(error.error);
      }
    });
  }
}
