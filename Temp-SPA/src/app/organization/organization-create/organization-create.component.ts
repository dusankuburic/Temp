import { Component, OnInit } from '@angular/core';
import { FormControl, UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { Organization } from 'src/app/core/models/organization';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { OrganizationService } from 'src/app/core/services/organization.service';
import { OrganizationValidators } from '../organization-validators';

@Component({
  selector: 'app-organization-create',
  templateUrl: './organization-create.component.html'
})
export class OrganizationCreateComponent implements OnInit {
  createOrganizationForm: UntypedFormGroup;
  organization: Organization;

  name = new FormControl('',[
    Validators.required,
    Validators.minLength(3),
    Validators.maxLength(60)],
    [this.validators.validateNameNotTaken()]);

  constructor(
    private organizationService: OrganizationService,
    private alertify: AlertifyService,
    private fb: UntypedFormBuilder,
    private validators: OrganizationValidators) { }

  ngOnInit(): void {
    this.createOrganizationForm = this.fb.group({
      name: this.name
    });
  }

  create(): void {
    this.organization = {...this.createOrganizationForm.value};
    this.organizationService.createOrganization(this.organization).subscribe({
      next: () => {
        this.alertify.success('Successfully created');
        this.createOrganizationForm.reset();
      },
      error: () => {
        this.alertify.error('Unable to create organization');
      }
    });
  }
}
