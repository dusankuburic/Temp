import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'tmp-datepicker',
  templateUrl: './tmp-datepicker.component.html',
  styleUrl: './tmp-datepicker.component.css'
})
export class TmpDatepickerComponent implements ControlValueAccessor {
  @Input() placeholder = '';
  @Input() label = '';
  bsConfig: Partial<BsDatepickerConfig>;
  
  constructor(@Self() public controlDir: NgControl) {
    this.controlDir.valueAccessor = this;

    this.bsConfig = {
      containerClass: 'theme-dark-blue'
    };
  }

  writeValue(obj: any): void {}
  registerOnChange(fn: any): void {}
  registerOnTouched(fn: any): void {}
  setDisabledState?(isDisabled: boolean): void {}

  get control(): FormControl {
    return this.controlDir.control as FormControl;
  }

}
