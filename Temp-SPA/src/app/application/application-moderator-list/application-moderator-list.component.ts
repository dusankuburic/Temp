import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ModeratorListApplication, UpdateApplicationRequest } from 'src/app/_models/application';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ApplicationService } from 'src/app/_services/application.service';

@Component({
  selector: 'app-application-moderator-list',
  templateUrl: './application-moderator-list.component.html'
})
export class ApplicationModeratorListComponent implements OnInit {
  applications: ModeratorListApplication[];
  appStatusRequest = {} as UpdateApplicationRequest;
  user: any;

  constructor(
    private route: ActivatedRoute,
    private applicationService: ApplicationService,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.applications = data['applications'];
    });

    this.user = JSON.parse(localStorage.getItem('user'));
  }

  updateStatus(applicationId: number, teamId: number): void {
    this.appStatusRequest.moderatorId = this.user.id;

    this.applicationService.updateApplicationStatus(applicationId, this.appStatusRequest).subscribe(() => {
      this.loadApplications(teamId);
      this.alertify.success('Successfully updated');
    }, error => {
      this.alertify.error(error.error);
    });
  }

  loadApplications(teamId: number): void {
    this.applicationService.getTeamApplicationsForModerator(teamId, this.user.id).toPromise().then((res: ModeratorListApplication[]) => {
      this.applications = res;
    }, error => {
      this.alertify.error(error.error);
    });
  }

}
