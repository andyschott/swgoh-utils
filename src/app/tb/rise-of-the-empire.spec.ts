import { TestBed } from '@angular/core/testing';

import { RiseOfTheEmpire } from './rise-of-the-empire';

describe('RiseOfTheEmpire', () => {
  let service: RiseOfTheEmpire;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RiseOfTheEmpire);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should return rewards for a star level with populated gear data', () => {
    const reward = service.getRewards(45);

    expect(reward.stars).toBe(45);
    expect(reward.guildEventCurrencyMk1).toBe(5000);
    expect(reward.fragmentedSignalData).toBe(29);
    expect(reward.finishers).toEqual([24, 24]);
    expect(reward.gear12PlusMain).toEqual([30, 29]);
  });

  it('should return null values for unknown reward columns at a valid star level', () => {
    const reward = service.getRewards(56);

    expect(reward.stars).toBe(56);
    expect(reward.fragmentedSignalData).toBeNull();
    expect(reward.incompleteSignalData).toBeNull();
    expect(reward.finishers).toBeNull();
    expect(reward.kyroKeypads).toBeNull();
  });

  it('should throw when rewards are not found for a star level', () => {
    expect(() => service.getRewards(0)).toThrowError('No rewards found for star level 0.');
  });
});
