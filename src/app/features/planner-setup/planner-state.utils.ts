import {
  PlannerRuleZone,
  ZoneCollapseStateMap,
  ZoneSlotStateChanges,
  ZoneStateMap,
} from './planner-types';

export const zoneUnlockDependency: Readonly<Record<string, string>> = {
  'south-back': 'south-front',
  ships: 'north',
};

export function buildEmptyZoneState(zones: ReadonlyArray<PlannerRuleZone>): ZoneStateMap {
  const state: ZoneStateMap = {};
  for (const zone of zones) {
    state[zone.zoneId] = createEmptyZoneSlotState(zone.slots);
  }

  return state;
}

export function syncZoneStateWithRules(
  previousState: ZoneStateMap,
  zones: ReadonlyArray<PlannerRuleZone>,
): ZoneStateMap {
  const nextState: ZoneStateMap = {};

  for (const zone of zones) {
    const existingState = previousState[zone.zoneId];
    nextState[zone.zoneId] = {
      teams: Array.from(
        { length: zone.slots },
        (_, index) => existingState?.teams[index] ?? '',
      ),
      defeated: Array.from(
        { length: zone.slots },
        (_, index) => existingState?.defeated[index] ?? false,
      ),
      attackOptions: Array.from(
        { length: zone.slots },
        (_, index) => existingState?.attackOptions[index] ?? [],
      ),
      attackOptionDrafts: Array.from(
        { length: zone.slots },
        (_, index) => existingState?.attackOptionDrafts[index] ?? '',
      ),
    };
  }

  return nextState;
}

export function syncZoneCollapsedStateWithRules(
  previousState: ZoneCollapseStateMap,
  zones: ReadonlyArray<PlannerRuleZone>,
): ZoneCollapseStateMap {
  const nextState: ZoneCollapseStateMap = {};
  for (const zone of zones) {
    nextState[zone.zoneId] = previousState[zone.zoneId] ?? false;
  }

  return nextState;
}

export function updateZoneSlotState(
  currentState: ZoneStateMap,
  zoneId: string,
  slotIndex: number,
  changes: ZoneSlotStateChanges,
): ZoneStateMap {
  const currentZone = currentState[zoneId];
  if (!currentZone) {
    return currentState;
  }

  const teams = currentZone.teams.map((team, index) =>
    index === slotIndex ? (changes.teamName ?? team) : team,
  );
  const defeated = currentZone.defeated.map((value, index) =>
    index === slotIndex ? (changes.defeated ?? value) : value,
  );
  const attackOptions = currentZone.attackOptions.map((options, index) =>
    index === slotIndex ? (changes.attackOptions ?? options) : options,
  );
  const attackOptionDrafts = currentZone.attackOptionDrafts.map((draft, index) =>
    index === slotIndex ? (changes.attackOptionDraft ?? draft) : draft,
  );

  return {
    ...currentState,
    [zoneId]: {
      teams,
      defeated,
      attackOptions,
      attackOptionDrafts,
    },
  };
}

export function areAllZoneTeamsDefeated(state: ZoneStateMap, zoneId: string): boolean {
  const zone = state[zoneId];
  if (!zone || zone.defeated.length === 0) {
    return false;
  }

  return zone.defeated.every(Boolean);
}

export function isSlotDefeated(state: ZoneStateMap, zoneId: string, slotIndex: number): boolean {
  return state[zoneId]?.defeated[slotIndex] ?? false;
}

function createEmptyZoneSlotState(slots: number) {
  return {
    teams: Array.from({ length: slots }, () => ''),
    defeated: Array.from({ length: slots }, () => false),
    attackOptions: Array.from({ length: slots }, () => [] as string[]),
    attackOptionDrafts: Array.from({ length: slots }, () => ''),
  };
}
