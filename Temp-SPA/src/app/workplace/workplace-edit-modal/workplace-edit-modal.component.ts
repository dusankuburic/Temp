import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { WorkplaceService } from 'src/app/core/services/workplace.service';
import { WorkplaceValidators } from '../workplace-validators';
import { Workplace } from 'src/app/core/models/workplace';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { takeUntil } from 'rxjs';
import { DestroyableComponent } from 'src/app/core/base/destroyable.component';

@Component({
  selector: 'workplace-edit-modal',
  templateUrl: './workplace-edit-modal.component.html'
})
export class WorkplaceEditModalComponent extends DestroyableComponent implements OnInit {
  editWorkplaceForm!: FormGroup;
  workplace!: Workplace;
  title?: string;
  workplaceId!: number;

  name = new FormControl('', [
    Validators.required, 
    Validators.minLength(3), 
    Validators.maxLength(60)]);

  constructor(
    private workplaceService: WorkplaceService,
    private fb: FormBuilder,
    private alertify: AlertifyService,
    private validators: WorkplaceValidators,
    public bsModalRef: BsModalRef) {
      super();
    }

    ngOnInit(): void {
      this.editWorkplaceForm = this.fb.group({
        name: this.name
      });

      this.workplaceService.getWorkplace(this.workplaceId).pipe(takeUntil(this.destroy$)).subscribe({
        next: (res) => {
          this.workplace = res;
          this.setupForm(this.workplace);
        },
        error: () => {
          this.alertify.error('Unable to get workplace');
        }
      });
    }

    setupForm(workplace: Workplace): void {
        this.editWorkplaceForm.patchValue({
          name: workplace.name
        });
      
      this.name.addAsyncValidators(this.validators.validateNameNotTaken(workplace.name))
    }
  
    update(): void {
      const workplaceForm = { ...this.editWorkplaceForm.value, id: this.workplace.id};
      this.workplaceService.updateWorkplace(workplaceForm).pipe(takeUntil(this.destroy$)).subscribe({
        next: () => {
          this.bsModalRef.content.isSaved = true;
          this.alertify.success('Successfully updated');
        },
        error: () => {
          this.alertify.error('Unable to edit workplace');
        }
      });
    }
}
