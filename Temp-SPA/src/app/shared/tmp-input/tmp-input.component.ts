import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';

@Component({
  selector: 'tmp-input',
  templateUrl: './tmp-input.component.html',
  styleUrl: './tmp-input.component.css'
})
export class TmpInputComponent implements ControlValueAccessor  {
  @Input() type = 'text';
  @Input() placeholder = '';
  @Input() label = '';

  constructor(@Self() public controlDir: NgControl) {
    this.controlDir.valueAccessor = this;
  }

  writeValue(obj: any): void {}
  registerOnChange(fn: any): void {}
  registerOnTouched(fn: any): void {}


  get control(): FormControl {
    return this.controlDir.control as FormControl
  }
}
