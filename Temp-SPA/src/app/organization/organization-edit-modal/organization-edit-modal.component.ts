import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { takeUntil } from 'rxjs';
import { Organization } from 'src/app/core/models/organization';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { OrganizationService } from 'src/app/core/services/organization.service';
import { OrganizationValidators } from '../organization-validators';
import { DestroyableComponent } from 'src/app/core/base/destroyable.component';
import { BlobDto, BlobResponse } from 'src/app/core/models/blob';

@Component({
    selector: 'app-organization-edit-modal',
    templateUrl: './organization-edit-modal.component.html',
    standalone: false
})
export class OrganizationEditModalComponent extends DestroyableComponent implements OnInit {
  editOrganizationForm!: FormGroup;
  organization!: Organization;
  title?: string;
  organizationId!: number;
  organizationFiles: BlobDto[] = [];
  profilePictureUrl?: string;

  name = new FormControl('', [
    Validators.required,
    Validators.minLength(3),
    Validators.maxLength(60)]);

  constructor(
    private organizationService: OrganizationService,
    private fb: FormBuilder,
    private alertify: AlertifyService,
    private validators: OrganizationValidators,
    public bsModalRef: BsModalRef) {
    super();
  }

  ngOnInit(): void {
    this.editOrganizationForm = this.fb.group({
      name: this.name
    });

    this.organizationService.getOrganization(this.organizationId).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res) => {
        this.organization = res;
        this.setupForm(this.organization);
      },
      error: () => {
        this.alertify.error('Unable to get organization');
      }
    });
  }

  setupForm(organization: Organization): void {
      this.editOrganizationForm.patchValue({
        name: organization.name
      });

      this.profilePictureUrl = organization.profilePictureUrl;
      this.name.addAsyncValidators(this.validators.validateNameNotTaken(organization.name))
  }

  update(): void {
    const request: Organization = {
      ...this.editOrganizationForm.value,
      id: this.organization.id,
      profilePictureUrl: this.profilePictureUrl
    };
    this.organizationService.updateOrganization(request).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.bsModalRef.content.isSaved = true;
        this.alertify.success('Successfully updated');
      },
      error: () => {
        this.alertify.error('Unable to update organization');
      }
    });
  }

  onFileUploaded(response: BlobResponse): void {
    if (!response.error && response.blob) {
      if (response.blob.fileType === 'Image') {
        this.profilePictureUrl = response.blob.name;
      }
      this.organizationFiles = [...this.organizationFiles, response.blob];
    }
  }

  onFileDeleted(path: string): void {
    if (path === this.profilePictureUrl) {
      this.profilePictureUrl = undefined;
    }
    this.organizationFiles = this.organizationFiles.filter(f => f.name !== path);
  }
}
