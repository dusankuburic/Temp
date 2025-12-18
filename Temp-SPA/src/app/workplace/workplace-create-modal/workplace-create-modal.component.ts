import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Workplace } from 'src/app/core/models/workplace';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { WorkplaceService } from 'src/app/core/services/workplace.service';
import { WorkplaceValidators } from 'src/app/workplace/workplace-validators';
import { takeUntil } from 'rxjs';
import { DestroyableComponent } from 'src/app/core/base/destroyable.component';

@Component({
  selector: 'workplace-create-modal',
  templateUrl: './workplace-create-modal.component.html'
})
export class WorkplaceCreateModalComponent extends DestroyableComponent implements OnInit {
  createWorkplaceForm!: FormGroup;
  workplace!: Workplace;
  title?: string;
  
  name = new FormControl('', [
    Validators.required, 
    Validators.minLength(3), 
    Validators.maxLength(60)], 
    [this.validators.validateNameNotTaken()]);

  constructor(
    private workplaceService: WorkplaceService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private validators: WorkplaceValidators,
    public bsModalRef: BsModalRef) {
      super();
    }
    

    ngOnInit(): void {
      this.createWorkplaceForm = this.fb.group({
        name: this.name
      });
    }

    create(): void {
      this.workplace = { ...this.createWorkplaceForm.value };
      this.workplaceService.createWorkplace(this.workplace).pipe(takeUntil(this.destroy$)).subscribe({
        next: () => {
          this.bsModalRef.content.isSaved = true;
          this.createWorkplaceForm.reset();
          this.alertify.success('Successfully created');
        },
        error: () => {
          this.alertify.error('Unable to create workplace');
        }
      });
    }
}
