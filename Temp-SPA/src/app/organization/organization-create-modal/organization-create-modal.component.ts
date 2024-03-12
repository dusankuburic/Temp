import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Organization } from 'src/app/core/models/organization';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { OrganizationService } from 'src/app/core/services/organization.service';
import { OrganizationValidators } from '../organization-validators';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-organization-create-modal',
  templateUrl: './organization-create-modal.component.html',
})
export class OrganizationCreateModalComponent implements OnInit {
  createOrganizationForm: FormGroup;
  organization: Organization;
  title?: string;

  name = new FormControl('',[
    Validators.required,
    Validators.minLength(3),
    Validators.maxLength(60)],
    [this.validators.validateNameNotTaken()]);

  constructor(
    private organizationService: OrganizationService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private validators: OrganizationValidators,
    public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
    this.createOrganizationForm = this.fb.group({
      name: this.name
    });
  }

  create(): void {
    this.organization = {...this.createOrganizationForm.value};
    this.organizationService.createOrganization(this.organization).subscribe({
      next: () => {
        this.bsModalRef.content.isSaved = true;
        this.alertify.success('Successfully created');
        this.createOrganizationForm.reset();
      },
      error: () => {
        this.alertify.error('Unable to create organization');
      }
    });
  }

}
