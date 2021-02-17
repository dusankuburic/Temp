import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Employee } from 'src/app/_models/employee';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { EmployeeService } from 'src/app/_services/employee.service';

@Component({
  selector: 'app-employee-edit',
  templateUrl: './employee-edit.component.html',
  styleUrls: ['./employee-edit.component.scss']
})
export class EmployeeEditComponent implements OnInit {
  editEmployeeForm: FormGroup;
  employee: Employee;

  constructor(
    private employeeService: EmployeeService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private alertify: AlertifyService,
    private router: Router) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.employee = data['employee'];
    });

    console.log(this.employee);

    this.createForm();

  }

  createForm(): void {
    this.editEmployeeForm = this.fb.group({
      firstName: [this.employee.firstName, Validators.required],
      lastName: [this.employee.lastName, Validators.required]
    });
  }

  update(): any {
    const employeeForm = Object.assign({}, this.editEmployeeForm.value);
    this.employeeService.updateEmployee(this.employee.id, employeeForm).subscribe(() => {
      this.alertify.success('Successfully updated');
    }, error => {
      this.alertify.error(error);
    })
  }
}
