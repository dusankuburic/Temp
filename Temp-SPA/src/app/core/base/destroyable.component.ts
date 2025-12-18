import { Component, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';

/**
 * Base component that provides automatic subscription cleanup
 *
 * Usage:
 * export class MyComponent extends DestroyableComponent {
 *   ngOnInit() {
 *     this.myService.getData()
 *       .pipe(takeUntil(this.destroy$))
 *       .subscribe(data => { });
 *   }
 * }
 */
@Component({
    template: '',
    standalone: false
})
export abstract class DestroyableComponent implements OnDestroy {
  protected destroy$ = new Subject<void>();

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
