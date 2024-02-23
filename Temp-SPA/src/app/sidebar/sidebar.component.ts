import { Component, OnInit } from '@angular/core';
import { AuthService } from '../core/services/auth.service';
import { faAddressBook, faAngleRight, faBriefcase, faClipboard, faFileAlt, faHotel, faIndustry, faUsers } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html'
})
export class SidebarComponent implements OnInit {
  arrowIcon = faAngleRight
  employeeIcon = faAddressBook
  workplaceIcon = faBriefcase
  employmentStatusIcon = faClipboard
  engagementIcon = faIndustry
  organizationIcon = faHotel
  applicationIcon = faFileAlt
  groupsIcon = faUsers

  user: any;
  constructor(private authService: AuthService) {}

  ngOnInit(): void {}

  loggedIn(): any {
    this.user = JSON.parse(localStorage.getItem('user'));
    return this.authService.loggedIn();
  }

  isAdmin(): boolean {
    return this.authService.decodedToken.role === 'Admin'? true : false;
  }

  isUser(): boolean {
    return this.authService.decodedToken.role === 'User' ? true : false;
  }

  isModerator(): boolean {
    return this.authService.decodedToken.role === 'Moderator' ? true : false;
  }
}
