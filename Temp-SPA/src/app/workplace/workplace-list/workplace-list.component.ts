import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Workplace } from 'src/app/_models/workplace';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { WorkplaceService } from 'src/app/_services/workplace.service';

@Component({
  selector: 'app-workplace-list',
  templateUrl: './workplace-list.component.html',
  styleUrls: ['./workplace-list.component.scss']
})
export class WorkplaceListComponent implements OnInit {
  workplaces: Workplace[];

  constructor(
    private route: ActivatedRoute,
    private workplaceService: WorkplaceService,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.workplaces = data['workplaces'];
    });
  }


}
