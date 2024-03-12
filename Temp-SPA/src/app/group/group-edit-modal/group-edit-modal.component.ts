import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Group } from 'src/app/core/models/group';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { GroupService } from 'src/app/core/services/group.service';
import { GroupValidators } from '../group-validators';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-group-edit-modal',
  templateUrl: './group-edit-modal.component.html'
})
export class GroupEditModalComponent implements OnInit {
  editGroupForm: FormGroup;
  group: Group;

  organizationId: number;
  groupId: number;
  title?: string;

  name = new FormControl('',[
    Validators.required,
    Validators.minLength(3),
    Validators.maxLength(60)
  ]);

  constructor(
    private groupService: GroupService,
    private fb: FormBuilder,
    private alertify: AlertifyService,
    public validators: GroupValidators,
    public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
    this.editGroupForm = this.fb.group({
      name: this.name
    });

    this.groupService.getGroup(this.groupId).subscribe({
      next: (res: Group) => {
        this.group = res;
        this.setupForm(this.group);
      },
      error: () => {
        this.alertify.error('Unable to get group');
      }
    });
  }

  setupForm(group: Group): void {
    this.editGroupForm.patchValue({
      name: group.name
    });

    this.name.addAsyncValidators(this.validators.validateNameNotTaken(this.organizationId, group.name));
  }

  update(): void {
    const groupForm = { ...this.editGroupForm.value };
    this.group.name = groupForm.name;
    this.groupService.updateGroup(this.group.id, this.group).subscribe({
      next: () => {
        this.bsModalRef.content.isSaved = true;
        this.alertify.success('Successfully updated');
      },
      error: () => {
        this.alertify.error('Unable to edit group');
      }
    });
  }
}
