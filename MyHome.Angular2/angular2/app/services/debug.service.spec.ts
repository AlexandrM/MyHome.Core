/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { DebugService } from './debug.service';

describe('DebugService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [DebugService]
    });
  });

  it('should ...', inject([DebugService], (service: DebugService) => {
    expect(service).toBeTruthy();
  }));
});
