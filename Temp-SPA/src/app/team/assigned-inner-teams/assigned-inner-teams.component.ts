import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { takeUntil } from 'rxjs';
import { faList } from '@fortawesome/free-solid-svg-icons';
import { InnerGroup } from 'src/app/core/models/group';
import { Pagination } from 'src/app/core/models/pagination';
import { InnerTeam } from 'src/app/core/models/team';
import { DestroyableComponent } from 'src/app/core/base/destroyable.component';

@Component({
  selector: 'app-assigned-inner-teams',
  templateUrl: './assigned-inner-teams.component.html'
})
export class AssignedInnerTeamsComponent extends DestroyableComponent implements OnInit {
  listIcon = faList;
  group: InnerGroup;
  innerTeams: InnerTeam[];
  pagination: Pagination;

  constructor(
    private route: ActivatedRoute) {
    super();
  }

  ngOnInit(): void {
    this.route.data.pipe(takeUntil(this.destroy$)).subscribe(data => {
      this.group = {
        id: data['innerteams'].id,
        name: data['innerteams'].name,
        hasActiveTeam: data['innerteams'].hasActiveTeam
      };
      this.innerTeams = data['innerteams'].teams.result;
      this.pagination = data['innerteams'].teams.pagination;
    });
  }

}
