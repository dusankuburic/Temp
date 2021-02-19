/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { WorkplaceService } from './workplace.service';

describe('Service: Workplace', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [WorkplaceService]
    });
  });

  it('should ...', inject([WorkplaceService], (service: WorkplaceService) => {
    expect(service).toBeTruthy();
  }));
});
