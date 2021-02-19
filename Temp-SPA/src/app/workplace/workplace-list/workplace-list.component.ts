import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Workplace } from 'src/app/_models/workplace';

@Component({
  selector: 'app-workplace-list',
  templateUrl: './workplace-list.component.html',
  styleUrls: ['./workplace-list.component.scss']
})
export class WorkplaceListComponent implements OnInit {
  workplaces: Workplace[];

  constructor(
    private router: ActivatedRoute,) { }

  ngOnInit(): void {
  }

}
