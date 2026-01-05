import {
  trigger,
  style,
  transition,
  query,
  animate,
  stagger
} from '@angular/animations';

export const tableAnimations = trigger('tableAnimations', [
  transition('* => *', [
    query(':enter', [
      style({ opacity: 0, transform: 'translateY(20px)' }),
      stagger('50ms', [
        animate('400ms cubic-bezier(0.2, 0.8, 0.2, 1)', style({ opacity: 1, transform: 'translateY(0)' }))
      ])
    ], { optional: true })
  ])
]);
