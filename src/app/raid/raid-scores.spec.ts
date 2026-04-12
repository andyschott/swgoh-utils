import { TestBed } from '@angular/core/testing';

import { RaidScores } from './raid-scores';

describe('RaidScores', () => {
  let service: RaidScores;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RaidScores);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should calculate total score from team counts by tier', () => {
    const score = service.calculateScore(3, 1);

    expect(score).toBe(1350000);
  });

  it('should calculate score for multiple higher tiers', () => {
    const score = service.calculateScore(undefined, undefined, 1, undefined, 2);

    expect(score).toBe(3000000);
  });

  it('should throw when a provided team count is zero', () => {
    expect(() => service.calculateScore(0, 1)).toThrow(RangeError);
  });

  it('should throw when a provided team count is negative', () => {
    expect(() => service.calculateScore(-1)).toThrow(RangeError);
  });

  it('should throw when a provided team count is not finite', () => {
    expect(() => service.calculateScore(Number.POSITIVE_INFINITY)).toThrow(TypeError);
  });

  it('should throw when total team count is less than one', () => {
    expect(() => service.calculateScore()).toThrow(RangeError);
  });

  it('should throw when total team count is greater than five', () => {
    expect(() => service.calculateScore(2, 2, 2)).toThrow(RangeError);
  });

  it('should calculate a team plan for a target score using lower tiers where possible', () => {
    const plan = service.calculateTeamsForTarget(1350000);

    expect(plan).toEqual({
      targetScore: 1350000,
      achievedScore: 1350000,
      totalTeams: 4,
      tier0Teams: 3,
      tier1Teams: 1,
      tier2Teams: 0,
      tier3Teams: 0,
      tier4Teams: 0,
      tier5Teams: 0,
      tier6Teams: 0,
      tier7Teams: 0,
    });
  });

  it('should find the lowest score that still reaches the target', () => {
    const plan = service.calculateTeamsForTarget(1000000);

    expect(plan.achievedScore).toBe(1050000);
    expect(plan.tier0Teams).toBe(2);
    expect(plan.tier1Teams).toBe(1);
  });

  it('should throw when target score is invalid', () => {
    expect(() => service.calculateTeamsForTarget(0)).toThrow(RangeError);
    expect(() => service.calculateTeamsForTarget(Number.POSITIVE_INFINITY)).toThrow(TypeError);
  });

  it('should throw when target score exceeds the maximum possible score', () => {
    expect(() => service.calculateTeamsForTarget(18000001)).toThrow(RangeError);
  });
});
