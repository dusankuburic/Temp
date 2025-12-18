import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { takeUntil } from 'rxjs';
import { Group } from '../../core/models/group';
import { faUsers } from '@fortawesome/free-solid-svg-icons';
import { DestroyableComponent } from 'src/app/core/base/destroyable.component';

@Component({
  selector: 'app-assigned-groups',
  templateUrl: './assigned-groups.component.html'
})
export class AssignedGroupsComponent extends DestroyableComponent implements OnInit {
  usersIcon = faUsers;
  groups!: Group[];
  constructor(private route: ActivatedRoute) {
    super();
  }

  ngOnInit(): void {
    this.route.data.pipe(takeUntil(this.destroy$)).subscribe(data => {
      this.groups = data['groups'];
    });
  }

}
