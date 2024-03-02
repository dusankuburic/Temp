import { Injectable } from "@angular/core";
import { AbstractControl, AsyncValidatorFn} from "@angular/forms";
import { debounceTime, finalize, map, switchMap, take } from "rxjs";
import { GroupService } from "../core/services/group.service";


@Injectable()
export class GroupValidators {
    constructor(private groupService: GroupService){}

    validateNameNotTaken(organizationId: number): AsyncValidatorFn {
        return (control: AbstractControl) => {
          return control.valueChanges.pipe(
            debounceTime(600),
            take(1),
            switchMap(() => {
              return this.groupService.checkGroupExists(control.value, organizationId).pipe(
                map(result => result ? {nameExists: true} : null),
                finalize(() => control.markAsTouched())
              )
            })
          )
        }
      }
}


