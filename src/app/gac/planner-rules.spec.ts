import { describe, expect, it } from 'vitest';
import { GacMode } from './gac-mode.enum';
import { League } from './league.enum';
import { getPlannerRules } from './planner-rules';

describe('getPlannerRules', () => {
  it('returns rules with selected mode and league', () => {
    const rules = getPlannerRules(GacMode.ThreeVThree, League.Aurodium);

    expect(rules.mode).toBe(GacMode.ThreeVThree);
    expect(rules.league).toBe(League.Aurodium);
  });

  it('applies league slot counts to ground and fleet zones', () => {
    const rules = getPlannerRules(GacMode.FiveVFive, League.Kyber);

    const groundZones = rules.zones.filter((zone) => zone.type === 'ground');
    const fleetZones = rules.zones.filter((zone) => zone.type === 'fleet');

    expect(groundZones.every((zone) => zone.slots === 5)).toBe(true);
    expect(fleetZones.every((zone) => zone.slots === 2)).toBe(true);
  });
});
