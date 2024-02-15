import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AssignRoleDto } from 'src/app/core/models/assignRoleDto';
import { Employee } from 'src/app/core/models/employee';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { EmployeeService } from 'src/app/core/services/employee.service';

@Component({
  selector: 'app-employee-assign-role',
  templateUrl: './employee-assign-role.component.html'
})
export class EmployeeAssignRoleComponent implements OnInit {
  createAssignRoleForm: UntypedFormGroup;
  employee: Employee;
  assignDto = {} as AssignRoleDto;


  constructor(
    private employeeService: EmployeeService,
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private fb: UntypedFormBuilder,
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

  passwordMatchValidator(form: UntypedFormGroup): any {
    return form.get('password').value === form.get('confirmPassword').value 
      ? null 
      : {mismatch: true};
  }

  register(): void {
    if (this.createAssignRoleForm.valid) {
      const formData = Object.assign({}, this.createAssignRoleForm.value);
      this.assignDto = {...formData};
      this.assignDto.id = this.employee.id;

      if (formData.role === 'Admin') {
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
