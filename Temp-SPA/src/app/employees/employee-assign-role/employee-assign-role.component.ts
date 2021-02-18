import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Admin } from 'src/app/_models/admin';
import { Employee } from 'src/app/_models/employee';
import { User } from 'src/app/_models/user';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-employee-assign-role',
  templateUrl: './employee-assign-role.component.html',
  styleUrls: ['./employee-assign-role.component.css']
})
export class EmployeeAssignRoleComponent implements OnInit {
  createAssignRoleForm: FormGroup;
  employee: Employee;
  admin = {} as Admin;
  user = {} as User;


  constructor(
    private authService: AuthService,
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
      role: ['user'],
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
      if (formData.role === 'admin'){

        this.admin.username = formData.username;
        this.admin.password = formData.password;
        this.admin.employeeId = this.employee.id;

        this.authService.registerAdmin(this.admin).subscribe(() => { 
          this.createAssignRoleForm.reset();
          this.router.navigate(['/employees']);
          this.alertify.success('Successfull admin registration');
        }, error => {
          this.alertify.error(error.error);
        });
      }
      else {
        this.user.username = formData.username;
        this.user.password = formData.password;
        this.user.employeeId = this.employee.id;

        this.authService.registerUser(this.user).subscribe(() => { 
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
