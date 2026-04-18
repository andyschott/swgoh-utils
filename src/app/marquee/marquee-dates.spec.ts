import { TestBed } from '@angular/core/testing';

import { MarqueeDates } from './marquee-dates';

describe('MarqueeDates', () => {
  let service: MarqueeDates;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MarqueeDates);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should return null when a character is not found', () => {
    expect(service.getDates('Unknown Character')).toBeNull();
  });

  it('should return dates when a character is found', () => {
    const dates = service.getDates('Zorii Bliss');
    expect(dates).not.toBeNull();
    expect(dates?.introduction?.toISOString().slice(0, 10)).toBe('2023-01-11');
  });

  it('should include known character names', () => {
    expect(service.getNames()).toContain('Zorii Bliss');
  });
});
