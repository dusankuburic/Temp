import { Injectable } from "@angular/core";
import { AbstractControl, AsyncValidatorFn} from "@angular/forms";
import { debounceTime, finalize, map, of, switchMap, take } from "rxjs";
import { OrganizationService } from "../core/services/organization.service";

@Injectable()
export class OrganizationValidators {
    constructor(private organizationService: OrganizationService){}

    validateNameNotTaken(initName?: string): AsyncValidatorFn {
        return (control: AbstractControl) => {
          return control.valueChanges.pipe(
            debounceTime(600),
            take(1),
            switchMap(() => {
              if (control.value === initName)
                return of(null);

              return this.organizationService.checkOrganizationExists(control.value).pipe(
                map(result => result ? {nameExists: true} : null),
                finalize(() => control.markAsTouched())
              )
            })
          )
        }
      }
}


