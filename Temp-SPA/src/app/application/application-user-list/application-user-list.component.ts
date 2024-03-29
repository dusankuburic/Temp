import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faComment, faEye } from '@fortawesome/free-solid-svg-icons';
import { UserListApplication } from 'src/app/core/models/application';


@Component({
  selector: 'app-application-user-list',
  templateUrl: './application-user-list.component.html'
})
export class ApplicationUserListComponent implements OnInit {
  commentIcon = faComment
  eyeIcon = faEye;

  applications: UserListApplication[];

  constructor(private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.applications = data['applications'];
    });
  }

}
