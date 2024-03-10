import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators, FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AssignRoleDto } from 'src/app/core/models/assignRoleDto';
import { Employee } from 'src/app/core/models/employee';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EmployeeService } from 'src/app/core/services/employee.service';
import { PasswordValidator } from 'src/app/shared/validators/password.validators';

@Component({
  selector: 'app-employee-assign-role',
  templateUrl: './employee-assign-role.component.html'
})
export class EmployeeAssignRoleComponent implements OnInit {
  createAssignRoleForm: FormGroup;
  employee: Employee;
  assignDto: AssignRoleDto;
  
  username = new FormControl('', [
    Validators.required,
    Validators.minLength(8),
    Validators.maxLength(50)]);

  password = new FormControl('', [
    Validators.required,
    Validators.minLength(8),
    Validators.maxLength(50)
  ]);

  confirmPassword = new FormControl('', [Validators.required]);

  constructor(
    private employeeService: EmployeeService,
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private router: Router) { }

  ngOnInit(): void {
    this.createAssignRoleForm = this.fb.group({
      role: ['', Validators.required],
      username: this.username,
      password: this.password,
      confirmPassword: this.confirmPassword
    });

    this.route.data.subscribe(data => {
      this.employee = data['employee'];
    });
    this.setupForm();
  }

  setupForm(): void {
    if (this.createAssignRoleForm)
      this.createAssignRoleForm.reset();

    this.createAssignRoleForm.get('role').setValue('User');
    this.createAssignRoleForm.addValidators([PasswordValidator.match('password', 'confirmPassword')]);
  }

  register(): void {
    if (this.createAssignRoleForm.valid) {
      this.assignDto = { ...this.createAssignRoleForm.value, id: this.employee.id };

      if (this.assignDto.role === 'Admin') {
        this.employeeService.assignRole(this.assignDto).subscribe(() => {
          this.createAssignRoleForm.reset();
          this.router.navigate(['/employees']);
          this.alertify.success('Successful admin registration');
        }, error => {
          this.alertify.error(error.error);
        });
      } else {
        this.employeeService.assignRole(this.assignDto).subscribe(() => {
          this.createAssignRoleForm.reset();
          this.router.navigate(['/employees']);
          this.alertify.success('Successful user registration');
        }, error => {
          this.alertify.error(error.error);
        });
      }
    }
  }

}
