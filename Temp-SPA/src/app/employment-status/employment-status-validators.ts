import { Injectable } from "@angular/core";
import { AbstractControl, AsyncValidatorFn} from "@angular/forms";
import { debounceTime, finalize, map, switchMap, take } from "rxjs";
import { EmploymentStatusService } from "../core/services/employment-status.service";


@Injectable()
export class EmploymentStatusValidators {
    constructor(private employmentStatusService: EmploymentStatusService){}

    validateNameNotTaken(): AsyncValidatorFn {
        return (control: AbstractControl) => {
          return control.valueChanges.pipe(
            debounceTime(600),
            take(1),
            switchMap(() => {
              return this.employmentStatusService.checkEmploymentStatusExists(control.value).pipe(
                map(result => result ? {nameExists: true} : null),
                finalize(() => control.markAsTouched())
              )
            })
          )
        }
      }
}


