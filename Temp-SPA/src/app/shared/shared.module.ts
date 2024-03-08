import { CommonModule } from "@angular/common";
import {  NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";
import { PaginationModule } from "ngx-bootstrap/pagination";
import { TmpPaginationComponent } from "./tmp-pagination/tmp-pagination.component";
import { TmpInputComponent } from "./tmp-input/tmp-input.component";
import { BsDatepickerModule } from "ngx-bootstrap/datepicker";

@NgModule({
    declarations: [
      TmpPaginationComponent,
      TmpInputComponent
    ],
    imports: [
      BsDatepickerModule,
      CommonModule,
      FormsModule,
      ReactiveFormsModule,
      PaginationModule,
      FontAwesomeModule
    ],
    exports: [
      BsDatepickerModule,
      CommonModule,
      FormsModule,
      ReactiveFormsModule,
      PaginationModule,
      FontAwesomeModule,
      TmpPaginationComponent,
      TmpInputComponent
    ]
  })
  export class SharedModule {}