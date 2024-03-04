import { Injectable } from "@angular/core";
import { AbstractControl, AsyncValidatorFn} from "@angular/forms";
import { debounceTime, finalize, map, of, switchMap, take } from "rxjs";
import { GroupService } from "../core/services/group.service";

@Injectable()
export class GroupValidators {
    constructor(private groupService: GroupService){}

    validateNameNotTaken(organizationId: number, initName?: string): AsyncValidatorFn {
        return (control: AbstractControl) => {
          return control.valueChanges.pipe(
            debounceTime(400),
            take(1),
            switchMap(() => {
              if (control.value === initName)
                return of(null);
              
              return this.groupService.checkGroupExists(control.value, organizationId).pipe(
                map(result => result ? {nameExists: true} : null),
                finalize(() => control.markAsTouched())
              )
            })
          )
        }
      }
}


