import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Organization } from 'src/app/_models/organization';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { OrganizationService } from 'src/app/_services/organization.service';

@Component({
  selector: 'app-organization-list',
  templateUrl: './organization-list.component.html',
  styleUrls: ['./organization-list.component.scss']
})
export class OrganizationListComponent implements OnInit {
  organizations: Organization[];

  constructor(
    private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.organizations = data['organizations'];
    });
  }

}
