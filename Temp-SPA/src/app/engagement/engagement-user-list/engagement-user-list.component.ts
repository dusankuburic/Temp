import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { takeUntil } from 'rxjs';
import { DestroyableComponent } from 'src/app/core/base/destroyable.component';
import { UserEngagement } from 'src/app/core/models/engagement';

@Component({
  selector: 'app-engagement-user-list',
  templateUrl: './engagement-user-list.component.html'
})
export class EngagementUserListComponent extends DestroyableComponent implements OnInit {
  engagements!: UserEngagement[];

  constructor(
    private route: ActivatedRoute) {
    super();
  }

  ngOnInit(): void {
    this.route.data.pipe(takeUntil(this.destroy$)).subscribe(data => {
        this.engagements = data['engagements'];
    });
  }
  

}
