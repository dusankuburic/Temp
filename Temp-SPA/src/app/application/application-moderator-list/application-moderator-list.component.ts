import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faBookmark, faComment, faEye } from '@fortawesome/free-solid-svg-icons';
import { ModeratorListApplication, UpdateApplicationRequest } from 'src/app/core/models/application';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { ApplicationService } from 'src/app/core/services/application.service';

@Component({
  selector: 'app-application-moderator-list',
  templateUrl: './application-moderator-list.component.html'
})
export class ApplicationModeratorListComponent implements OnInit {
  eyeIcon = faEye;
  commentIcon = faComment
  bookmarkIcon = faBookmark;
  applications: ModeratorListApplication[];
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
    const request: UpdateApplicationRequest = {
      id: applicationId, 
      moderatorId: this.user.id
    };

    this.applicationService.updateApplicationStatus(request).subscribe(() => {
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
