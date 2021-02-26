import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Group } from 'src/app/_models/group';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { GroupService } from 'src/app/_services/group.service';

@Component({
  selector: 'app-group-edit',
  templateUrl: './group-edit.component.html',
  styleUrls: ['./group-edit.component.scss']
})
export class GroupEditComponent implements OnInit {
  editGroupForm: FormGroup;
  group: Group;

  constructor(
    private groupService: GroupService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.group = data['group'];
    });
    this.createForm();
  }

  createForm(): void {
    this.editGroupForm = this.fb.group({
      Name: [this.group.name, Validators.required]
    });
  }

  update(): void {
    const groupForm = Object.assign({}, this.editGroupForm.value);
    this.group.name = groupForm.Name;
    this.groupService.updateGroup(this.group.id, this.group).subscribe(() => {
      this.alertify.success('Successfully updated');
    }, error => {
      this.alertify.error(error.error);
    });
  }

}
