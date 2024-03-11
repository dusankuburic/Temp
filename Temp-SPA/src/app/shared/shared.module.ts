import { CommonModule } from "@angular/common";
import {  NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";
import { PaginationModule } from "ngx-bootstrap/pagination";
import { TmpPaginationComponent } from "./components/tmp-pagination/tmp-pagination.component";
import { TmpInputComponent } from "./components/tmp-input/tmp-input.component";
import { BsDatepickerModule } from "ngx-bootstrap/datepicker";
import { PasswordValidator } from "./validators/password.validators";
import { TmpDatepickerComponent } from "./components/tmp-datepicker/tmp-datepicker.component";
import { ControlValueAccessorDirective } from "./components/control-value-accessor.directive";
import { TmpSelectComponent } from "./components/tmp-select/tmp-select.component";
import { BsModalService } from "ngx-bootstrap/modal";

@NgModule({
    declarations: [
      ControlValueAccessorDirective,
      TmpPaginationComponent,
      TmpInputComponent,
      TmpDatepickerComponent,
      TmpSelectComponent,
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
      CommonModule,
      FormsModule,
      ReactiveFormsModule,
      PaginationModule,
      FontAwesomeModule,
      TmpPaginationComponent,
      TmpInputComponent,
      TmpDatepickerComponent,
      TmpSelectComponent
    ],
    providers: [
      PasswordValidator,
      BsModalService,
    ]
  })
  export class SharedModule {}