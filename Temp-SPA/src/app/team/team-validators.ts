import { Injectable } from "@angular/core";
import { AbstractControl, AsyncValidatorFn} from "@angular/forms";
import { debounceTime, finalize, map, of, switchMap, take } from "rxjs";
import { TeamService } from "../core/services/team.service";

@Injectable()
export class TeamValidators {
    constructor(private teamService: TeamService){}

    validateNameNotTaken(groupId: number, initName?: string): AsyncValidatorFn {
        return (control: AbstractControl) => {
          return control.valueChanges.pipe(
            debounceTime(400),
            take(1),
            switchMap(() => {
              if (control.value === initName)
                return of(null);

              return this.teamService.checkTeamExists(control.value, groupId).pipe(
                map(result => result ? {nameExists: true} : null),
                finalize(() => control.markAsTouched())
              )
            })
          )
        }
      }
}


