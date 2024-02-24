import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";
import { PaginationModule } from "ngx-bootstrap/pagination";

@NgModule({
    imports: [
      CommonModule,
      FormsModule,
      ReactiveFormsModule,
      PaginationModule.forRoot(),
      FontAwesomeModule
    ],
    declarations: [],
    exports: [
      CommonModule,
      FormsModule,
      ReactiveFormsModule,
      PaginationModule,
      FontAwesomeModule,
    ]
  })
  export class SharedModule { }