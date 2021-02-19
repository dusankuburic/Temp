import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Employee } from 'src/app/_models/employee';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { EmployeeService } from 'src/app/_services/employee.service';

@Component({
  selector: 'app-employee-create',
  templateUrl: './employee-create.component.html',
  styleUrls: ['./employee-create.component.scss']
})
export class EmployeeCreateComponent implements OnInit {
  createEmployeeForm: FormGroup;
  employee: Employee;

  constructor(
    private employeeService: EmployeeService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private router: Router) { }

  ngOnInit(): void {
    this.createForm();
  }

  createForm(): void {
    this.createEmployeeForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required]
    });
  }

  create(): any {
    this.employee = Object.assign({}, this.createEmployeeForm.value);
    this.employeeService.createEmployee(this.employee).subscribe(() => {
     // this.router.navigate(['employee/create']);
      this.alertify.success('Successfully created');
      this.createEmployeeForm.reset();
    }, error => {
      this.alertify.error(error);
    });
  }

}
