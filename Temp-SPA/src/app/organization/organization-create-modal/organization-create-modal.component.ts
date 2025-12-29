import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { takeUntil, finalize } from 'rxjs';
import { Organization } from 'src/app/core/models/organization';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { OrganizationService } from 'src/app/core/services/organization.service';
import { UploadService } from 'src/app/core/services/upload.service';
import { OrganizationValidators } from '../organization-validators';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { DestroyableComponent } from 'src/app/core/base/destroyable.component';

@Component({
    selector: 'app-organization-create-modal',
    templateUrl: './organization-create-modal.component.html',
    standalone: false
})
export class OrganizationCreateModalComponent extends DestroyableComponent implements OnInit {
  createOrganizationForm!: FormGroup;
  organization!: Organization;
  title?: string;
  profilePictureUrl?: string;
  isUploading = false;

  name = new FormControl('',[
    Validators.required,
    Validators.minLength(3),
    Validators.maxLength(60)],
    [this.validators.validateNameNotTaken()]);

  constructor(
    private organizationService: OrganizationService,
    private uploadService: UploadService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private validators: OrganizationValidators,
    public bsModalRef: BsModalRef) {
    super();
  }

  ngOnInit(): void {
    this.createOrganizationForm = this.fb.group({
      name: this.name
    });
  }

  create(): void {
    if (this.isUploading) return;

    this.organization = {
      ...this.createOrganizationForm.value,
      profilePictureUrl: this.profilePictureUrl
    };
    this.organizationService.createOrganization(this.organization).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.bsModalRef.content.isSaved = true;
        this.alertify.success('Successfully created');
        this.createOrganizationForm.reset();
        this.profilePictureUrl = undefined;
      },
      error: () => {
        this.alertify.error('Unable to create organization');
      }
    });
  }

  onFileSelected(event: any): void {
    const file: File = event.target.files[0];
    if (file) {
      this.isUploading = true;
      this.uploadService.uploadFile(file)
        .pipe(finalize(() => this.isUploading = false))
        .subscribe({
          next: () => {
             this.profilePictureUrl = `http://127.0.0.1:10000/devstoreaccount1/uploads/${file.name}`;
             this.alertify.success('Image uploaded successfully');
          },
          error: () => {
            this.alertify.error('Image upload failed');
          }
        });
    }
  }
}
