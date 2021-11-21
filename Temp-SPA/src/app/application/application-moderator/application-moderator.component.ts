import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Application } from "src/app/_models/application";


@Component({
    selector: 'app-application-moderator',
    templateUrl: './application-user.component.html'
})
export class ApplicationModeratorComponent implements OnInit {
    application: Application;

    constructor(private route: ActivatedRoute) {}

    ngOnInit(): void {
        this.route.data.subscribe(data => {
            this.application = data['application'];
        });
    }
}