import { Component, Input, forwardRef } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { ControlValueAccessorDirective } from '../control-value-accessor.directive';

export interface SelectionOption {
  value: any, 
  display: any,
  disabled?: boolean,
  selected?: boolean,
  hidden?: boolean
};

@Component({
  selector: 'tmp-select',
  templateUrl: './tmp-select.component.html',
  styleUrl: './tmp-select.component.css',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => TmpSelectComponent),
      multi: true,
    }
  ]
})
export class TmpSelectComponent<T> extends ControlValueAccessorDirective<T> {
  @Input() options: SelectionOption[] = [];
  @Input() label = '';
  @Input() customErrorMessages: Record<string, string> = {};
}
