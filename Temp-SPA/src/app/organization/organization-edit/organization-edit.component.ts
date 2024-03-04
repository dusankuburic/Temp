import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Organization } from 'src/app/core/models/organization';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { OrganizationService } from 'src/app/core/services/organization.service';

@Component({
  selector: 'app-organization-edit',
  templateUrl: './organization-edit.component.html'
})
export class OrganizationEditComponent implements OnInit {
  editOrganizationForm: UntypedFormGroup;
  organization: Organization;

  name = new FormControl('', [
    Validators.required,
    Validators.minLength(3),
    Validators.maxLength(60)
  ]);

  constructor(
    private organizationService: OrganizationService,
    private route: ActivatedRoute,
    private fb: UntypedFormBuilder,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.editOrganizationForm = this.fb.group({
      name: this.name
    });

    this.route.data.subscribe(data => {
      this.organization = data['organization'];
      this.setupForm(this.organization);
    });
  }

  setupForm(organization: Organization): void {
    if (this.editOrganizationForm)
      this.editOrganizationForm.reset();

      this.editOrganizationForm.patchValue({
        name: organization.name
      });
  }

  update(): void {
    const request: Organization = {...this.editOrganizationForm.value, id: this.organization.id};
    this.organizationService.updateOrganization(request).subscribe({
      next: () => {
        this.alertify.success('Successfully updated');
      },
      error: (error) => {
        this.alertify.error(error.error);
      }
    });
  }
}
