import { Component, Input, forwardRef } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { ControlValueAccessorDirective } from '../control-value-accessor.directive';
import { faCheck, faExclamationCircle } from '@fortawesome/free-solid-svg-icons';

type InputType = 'text' | 'number' | 'email' | 'password';

let nextUniqueId = 0;

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
    ],
    standalone: false
})
export class TmpInputComponent<T> extends ControlValueAccessorDirective<T> {
  @Input() type: InputType = 'text';
  @Input() placeholder = '';
  @Input() label = '';
  @Input() isFilter: boolean = false;

  isFocused = false;
  private uniqueId = `tmp-input-${++nextUniqueId}`;

  get inputId(): string {
    return this.uniqueId;
  }

  get errorId(): string {
    return `${this.uniqueId}-error`;
  }

  onFocus(): void {
    this.isFocused = true;
  }

  onBlur(): void {
    this.isFocused = false;
  }

  // Icons
  protected readonly faCheck = faCheck;
  protected readonly faExclamationCircle = faExclamationCircle;
}