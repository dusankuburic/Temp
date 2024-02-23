import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faEdit, faPenNib, faProjectDiagram, faUsers } from '@fortawesome/free-solid-svg-icons';
import { Organization } from 'src/app/core/models/organization';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { OrganizationService } from 'src/app/core/services/organization.service';

@Component({
  selector: 'app-organization-list',
  templateUrl: './organization-list.component.html'
})
export class OrganizationListComponent implements OnInit {
  editOrganizationIcon = faEdit
  archiveOrganizationIcon = faPenNib
  innerGroupsIcon = faUsers
  createGroupIcon = faProjectDiagram

  organizations: Organization[];

  constructor(
    private route: ActivatedRoute,
    private organizationsService: OrganizationService,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.organizations = data['organizations'];
    });
  }

  loadOrganizations(): void {
    this.organizationsService.getOrganizations()
    .toPromise().then((result) => {
      this.organizations = result;
    }, error => {
      this.alertify.error(error.error);
    })
  }

  changeStatus(id: number): void {
    this.organizationsService.changeStatus({id}).subscribe({
      next: () => {
        this.loadOrganizations();
        this.alertify.success('Status changed');
      },
      error: (error) => {
        this.alertify.error(error.error);
      }
    });
  }

}
