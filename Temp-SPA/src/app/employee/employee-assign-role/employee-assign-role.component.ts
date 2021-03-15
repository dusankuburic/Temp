import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AssignRoleDto } from 'src/app/_models/assignRoleDto';
import { Employee } from 'src/app/_models/employee';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { EmployeeService } from 'src/app/_services/employee.service';

@Component({
  selector: 'app-employee-assign-role',
  templateUrl: './employee-assign-role.component.html'
})
export class EmployeeAssignRoleComponent implements OnInit {
  createAssignRoleForm: FormGroup;
  employee: Employee;
  assignDto = {} as AssignRoleDto;


  constructor(
    private employeeService: EmployeeService,
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private router: Router) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.employee = data['employee'];
    });
    this.createForm();
  }

  createForm(): void {
    this.createAssignRoleForm = this.fb.group({
      role: ['User'],
      username: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(30)]],
      confirmPassword: ['', Validators.required]
    }, {validator: this.passwordMatchValidator});
  }

  passwordMatchValidator(form: FormGroup): any {
    return form.get('password').value === form.get('confirmPassword').value ? null : {mismatch: true};
  }

  register(): any {
    if (this.createAssignRoleForm.valid)
    {
      const formData = Object.assign({}, this.createAssignRoleForm.value);
      this.assignDto.id = this.employee.id;
      this.assignDto.username = formData.username;
      this.assignDto.password = formData.password;
      this.assignDto.role = formData.role;

      if (formData.role === 'Admin'){
        this.employeeService.assignRole(this.assignDto).subscribe(() => {
          this.createAssignRoleForm.reset();
          this.router.navigate(['/employees']);
          this.alertify.success('Successfull admin registration');
        }, error => {
          this.alertify.error(error.error);
        });
      }
      else {
        this.employeeService.assignRole(this.assignDto).subscribe(() => {
          this.createAssignRoleForm.reset();
          this.router.navigate(['/employees']);
          this.alertify.success('Successfull user registration');
        }, error => {
          this.alertify.error(error.error);
        });
      }
    }
  }

}
