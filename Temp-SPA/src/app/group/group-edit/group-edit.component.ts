import { Component, OnInit } from '@angular/core';
import { FormControl, UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Group } from 'src/app/core/models/group';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { GroupService } from 'src/app/core/services/group.service';

@Component({
  selector: 'app-group-edit',
  templateUrl: './group-edit.component.html'
})
export class GroupEditComponent implements OnInit {
  editGroupForm: UntypedFormGroup;
  group: Group;

  constructor(
    private groupService: GroupService,
    private route: ActivatedRoute,
    private fb: UntypedFormBuilder,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.group = data['group'];
    });
    this.createForm();
  }

  name = new FormControl('',[
    Validators.required,
    Validators.minLength(3),
    Validators.maxLength(60)
  ]);

  createForm(): void {
    this.editGroupForm = this.fb.group({
      name: this.name.setValue(this.group.name)
    });
  }

  update(): void {
    //TODO: rewrite this
    const groupForm = { ...this.editGroupForm.value };
    this.group.name = groupForm.Name;
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
