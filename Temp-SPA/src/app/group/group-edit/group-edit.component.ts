import { Component, OnInit } from '@angular/core';
import { FormControl, UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Group } from 'src/app/core/models/group';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { GroupService } from 'src/app/core/services/group.service';
import { GroupValidators } from '../group-validators';

@Component({
  selector: 'app-group-edit',
  templateUrl: './group-edit.component.html'
})
export class GroupEditComponent implements OnInit {
  editGroupForm: UntypedFormGroup;
  group: Group;
  organizationId: number;

  name = new FormControl('',[
    Validators.required,
    Validators.minLength(3),
    Validators.maxLength(60)
  ]);

  constructor(
    private groupService: GroupService,
    private route: ActivatedRoute,
    private fb: UntypedFormBuilder,
    private alertify: AlertifyService,
    public validators: GroupValidators) { }

  ngOnInit(): void {
    this.editGroupForm = this.fb.group({
      name: this.name
    });

    this.organizationId = parseInt(this.route.snapshot.paramMap.get('organizationId'));

    this.route.data.subscribe(data => {
      this.group = data['group'];
      this.setupForm(this.group);
    });
  }

  setupForm(group: Group): void {
    if (this.editGroupForm)
      this.editGroupForm.reset();

      this.name.addAsyncValidators(this.validators.validateNameNotTaken(this.organizationId, group.name));

      this.editGroupForm.patchValue({
        name: group.name
      });
  }

  update(): void {
    const groupForm = { ...this.editGroupForm.value };
    this.group.name = groupForm.name;
    this.groupService.updateGroup(this.group.id, this.group).subscribe({
      next: () => {
        this.alertify.success('Successfully updated');
      },
      error: (error) => {
        this.alertify.error(error.error);
      }
    });
  }

}
