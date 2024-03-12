import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Group } from 'src/app/core/models/group';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { GroupService } from 'src/app/core/services/group.service';
import { GroupValidators } from '../group-validators';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-group-create-modal',
  templateUrl: './group-create-modal.component.html'
})
export class GroupCreateModalComponent {
  createGroupForm: FormGroup;
  group: Group;
  organizationId: number;
  title?: string;

  name = new FormControl('', [
    Validators.required,
    Validators.minLength(3),
    Validators.maxLength(60)]);

  constructor(
    private groupService: GroupService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private validators: GroupValidators,
    public bsModalRef: BsModalRef) { }


    ngOnInit(): void {
      this.createGroupForm = this.fb.group({
        name: this.name
      });

      this.name.setAsyncValidators(this.validators.validateNameNotTaken(this.organizationId))
    }
  
    create(): void {
      this.group = {...this.createGroupForm.value, organizationId: this.organizationId };
      this.groupService.createGroup(this.group).subscribe({
        next: () => {
          this.bsModalRef.content.isSaved = true;
          this.alertify.success('Successfully created');
          this.createGroupForm.reset();
        },
        error: () => {
          this.alertify.error('Unable to create group');
        }
      });
    }
}
