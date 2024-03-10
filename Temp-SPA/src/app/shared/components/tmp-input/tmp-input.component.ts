import { Component, Input, forwardRef } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { ControlValueAccessorDirective } from '../control-value-accessor.directive';

type InputType = 'text' | 'number' | 'email' | 'password';

@Component({
  selector: 'tmp-input',
  templateUrl: './tmp-input.component.html',
  styleUrl: './tmp-input.component.css',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => TmpInputComponent),
      multi: true
    }
  ]
})
export class TmpInputComponent<T> extends ControlValueAccessorDirective<T> { 
  @Input() type: InputType = 'text';
  @Input() placeholder = '';
  @Input() label = '';
  @Input() isFilter: boolean = false;
  @Input() customErrorMessages: Record<string, string> = {};
}