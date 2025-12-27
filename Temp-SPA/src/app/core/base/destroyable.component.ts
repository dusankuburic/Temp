import { Component, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';


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
