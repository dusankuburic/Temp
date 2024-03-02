import { Injectable } from "@angular/core";
import { AbstractControl, AsyncValidatorFn} from "@angular/forms";
import { debounceTime, finalize, map, switchMap, take } from "rxjs";
import { WorkplaceService } from "src/app/core/services/workplace.service";

@Injectable()
export class WorkplaceValidators {
    constructor(private workplaceService: WorkplaceService){}

    validateNameNotTaken(): AsyncValidatorFn {
        return (control: AbstractControl) => {
          return control.valueChanges.pipe(
            debounceTime(600),
            take(1),
            switchMap(() => {
              return this.workplaceService.checkWorkplaceExists(control.value).pipe(
                map(result => result ? {nameExists: true} : null),
                finalize(() => control.markAsTouched())
              )
            })
          )
        }
      }
}


