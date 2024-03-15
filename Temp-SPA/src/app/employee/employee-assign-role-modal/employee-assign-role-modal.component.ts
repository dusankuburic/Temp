import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AssignRoleDto } from 'src/app/core/models/assignRoleDto';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EmployeeService } from 'src/app/core/services/employee.service';
import { PasswordValidator } from 'src/app/shared/validators/password.validators';

@Component({
  selector: 'app-employee-assign-role-modal',
  templateUrl: './employee-assign-role-modal.component.html'
})
export class EmployeeAssignRoleModalComponent implements OnInit {

  title?: string;
  firstName: string;
  lastName: string;
  employeeId: number;
  createAssignRoleForm: FormGroup;
  assignDto: AssignRoleDto;
  
  username = new FormControl('', [
    Validators.required,
    Validators.minLength(8),
    Validators.maxLength(50)]);

  email = new FormControl('', [
    Validators.required,
    Validators.minLength(8),
    Validators.maxLength(50)]);

  password = new FormControl('', [
    Validators.required,
    Validators.minLength(8),
    Validators.maxLength(50)]);

  confirmPassword = new FormControl('', [Validators.required]);

  constructor(
    private employeeService: EmployeeService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
    this.createAssignRoleForm = this.fb.group({
      role: ['', Validators.required],
      email: this.email,
      username: this.username,
      password: this.password,
      confirmPassword: this.confirmPassword
    });

    this.setupForm();
  }

  setupForm(): void {
    this.createAssignRoleForm.get('role').setValue('User');
    this.createAssignRoleForm.addValidators([PasswordValidator.match('password', 'confirmPassword')]);
  }

  register(): void {
    if (this.createAssignRoleForm.valid) {
      this.assignDto = { ...this.createAssignRoleForm.value, id: this.employeeId };
      this.employeeService.assignRole(this.assignDto).subscribe(() => {
        this.bsModalRef.content.isSaved = true;
        this.createAssignRoleForm.disable();
        this.alertify.success('Successful user registration');
        this.bsModalRef.hide();
      }, error => {
        this.alertify.error(error.error);
      });
    }
  }
}
