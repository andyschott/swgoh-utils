import { describe, expect, it } from 'vitest';
import {
  buildEmptyZoneState,
  syncZoneCollapsedStateWithRules,
  syncZoneStateWithRules,
  updateZoneSlotState,
} from './planner-state.utils';

const zones = [
  { zoneId: 'north', label: 'North', slots: 2, type: 'Squad' },
  { zoneId: 'ships', label: 'Ships', slots: 1, type: 'Fleet' },
] as const;

describe('planner-state utils', () => {
  it('should build empty zone state with expected slot shapes', () => {
    const state = buildEmptyZoneState(zones);

    expect(state['north']).toEqual({
      teams: ['', ''],
      defeated: [false, false],
      attackOptions: [[], []],
      attackOptionDrafts: ['', ''],
    });
    expect(state['ships']).toEqual({
      teams: [''],
      defeated: [false],
      attackOptions: [[]],
      attackOptionDrafts: [''],
    });
  });

  it('should preserve existing slot values while syncing to new rules', () => {
    const previous = {
      north: {
        teams: ['Reva'],
        defeated: [true],
        attackOptions: [['Leia']],
        attackOptionDrafts: ['Typed'],
      },
    };

    const state = syncZoneStateWithRules(previous, zones);

    expect(state['north']).toEqual({
      teams: ['Reva', ''],
      defeated: [true, false],
      attackOptions: [['Leia'], []],
      attackOptionDrafts: ['Typed', ''],
    });
    expect(state['ships']).toEqual({
      teams: [''],
      defeated: [false],
      attackOptions: [[]],
      attackOptionDrafts: [''],
    });
  });

  it('should only update one slot when applying slot state changes', () => {
    const initial = buildEmptyZoneState(zones);

    const updated = updateZoneSlotState(initial, 'north', 1, {
      teamName: 'Jabba',
      defeated: true,
      attackOptions: ['Bane'],
      attackOptionDraft: 'Inqs',
    });

    expect(updated['north'].teams).toEqual(['', 'Jabba']);
    expect(updated['north'].defeated).toEqual([false, true]);
    expect(updated['north'].attackOptions).toEqual([[], ['Bane']]);
    expect(updated['north'].attackOptionDrafts).toEqual(['', 'Inqs']);
  });

  it('should keep collapse state for known zones and default new zones to expanded', () => {
    const collapsed = syncZoneCollapsedStateWithRules({ north: true }, zones);

    expect(collapsed).toEqual({ north: true, ships: false });
  });
});
