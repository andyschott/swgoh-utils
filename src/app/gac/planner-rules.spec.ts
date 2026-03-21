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

    const characterZones = rules.zones.filter((zone) => zone.type === 'characters');
    const shipZones = rules.zones.filter((zone) => zone.type === 'ships');
    const byZone = Object.fromEntries(rules.zones.map((zone) => [zone.zoneId, zone.slots]));

    expect(characterZones).toHaveLength(3);
    expect(shipZones).toHaveLength(1);
    expect(byZone['north']).toBe(4);
    expect(byZone['south-front']).toBe(4);
    expect(byZone['south-back']).toBe(3);
    expect(byZone['ships']).toBe(3);
  });

  it('uses the required four-zone layout names', () => {
    const rules = getPlannerRules(GacMode.ThreeVThree, League.Carbonite);
    const labels = rules.zones.map((zone) => zone.label);

    expect(labels).toEqual(['North', 'Ships', 'South Front', 'South Back']);
  });

  it('matches the requested 5v5 slot matrix by league', () => {
    const expectedByLeague: Record<
      League,
      { north: number; southFront: number; southBack: number; ships: number }
    > = {
      [League.Carbonite]: { north: 1, southFront: 1, southBack: 1, ships: 1 },
      [League.Bronzium]: { north: 2, southFront: 2, southBack: 1, ships: 1 },
      [League.Chromium]: { north: 3, southFront: 2, southBack: 2, ships: 2 },
      [League.Aurodium]: { north: 3, southFront: 3, southBack: 3, ships: 2 },
      [League.Kyber]: { north: 4, southFront: 4, southBack: 3, ships: 3 },
    };

    for (const league of Object.values(League)) {
      const rules = getPlannerRules(GacMode.FiveVFive, league);
      const byZone = Object.fromEntries(rules.zones.map((zone) => [zone.zoneId, zone.slots]));
      const expected = expectedByLeague[league];

      expect(byZone['north']).toBe(expected.north);
      expect(byZone['south-front']).toBe(expected.southFront);
      expect(byZone['south-back']).toBe(expected.southBack);
      expect(byZone['ships']).toBe(expected.ships);
    }
  });

  it('matches the requested 3v3 slot matrix by league', () => {
    const expectedByLeague: Record<
      League,
      { north: number; southFront: number; southBack: number; ships: number }
    > = {
      [League.Carbonite]: { north: 1, southFront: 1, southBack: 1, ships: 1 },
      [League.Bronzium]: { north: 2, southFront: 3, southBack: 2, ships: 1 },
      [League.Chromium]: { north: 3, southFront: 4, southBack: 3, ships: 2 },
      [League.Aurodium]: { north: 4, southFront: 5, southBack: 4, ships: 2 },
      [League.Kyber]: { north: 5, southFront: 5, southBack: 5, ships: 3 },
    };

    for (const league of Object.values(League)) {
      const rules = getPlannerRules(GacMode.ThreeVThree, league);
      const byZone = Object.fromEntries(rules.zones.map((zone) => [zone.zoneId, zone.slots]));
      const expected = expectedByLeague[league];

      expect(byZone['north']).toBe(expected.north);
      expect(byZone['south-front']).toBe(expected.southFront);
      expect(byZone['south-back']).toBe(expected.southBack);
      expect(byZone['ships']).toBe(expected.ships);
    }
  });
});
