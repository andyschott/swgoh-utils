import { TestBed } from '@angular/core/testing';

import { SignalDataFarming } from './signal-data-farming';

describe('SignalDataFarming', () => {
  let service: SignalDataFarming;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SignalDataFarming);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should return the expected fastest node path in order', () => {
    const result = service.getRemainingDays(1, 1, 1);

    expect(result.nodesToFarm).toEqual(['9-D', '9-F', '8-F']);
  });

  it('should return 0 days when no signal data is needed', () => {
    const result = service.getRemainingDays(0, 0, 0);

    expect(result.days).toBe(0);
  });

  it('should round up fractional day totals to the next whole day', () => {
    const result = service.getRemainingDays(1, 0, 0);

    expect(result.days).toBe(1);
  });
});
