import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Organization } from 'src/app/_models/organization';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { OrganizationService } from 'src/app/_services/organization.service';

@Component({
  selector: 'app-organization-edit',
  templateUrl: './organization-edit.component.html',
  styleUrls: ['./organization-edit.component.scss']
})
export class OrganizationEditComponent implements OnInit {
  editOrganizationForm: FormGroup;
  organization: Organization;

  constructor(
    private organizationService: OrganizationService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.organization = data['organization'];
    });
    this.createForm();
  }

  createForm(): void {
    this.editOrganizationForm = this.fb.group({
      Name: [this.organization.name, Validators.required]
    });
  }

  update(): void {
    const organizatonForm = Object.assign({}, this.editOrganizationForm.value);
    this.organizationService.updateOrganization(this.organization.id, organizatonForm).subscribe(() => {
      this.alertify.success('Successfully updated');
    }, error => {
      this.alertify.error(error.error);
    });
  }

}
