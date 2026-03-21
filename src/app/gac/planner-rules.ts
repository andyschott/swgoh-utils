import { GacMode } from './gac-mode.enum';
import { League } from './league.enum';

export type ZoneType = 'ground' | 'fleet';

export interface ZoneRequirement {
  zoneId: string;
  label: string;
  type: ZoneType;
  slots: number;
}

export interface PlannerRules {
  mode: GacMode;
  league: League;
  zones: readonly ZoneRequirement[];
}

const groundSlotsByLeague: Readonly<Record<League, number>> = {
  [League.Carbonite]: 3,
  [League.Bronzium]: 4,
  [League.Chromium]: 4,
  [League.Aurodium]: 5,
  [League.Kyber]: 5,
};

const fleetSlotsByLeague: Readonly<Record<League, number>> = {
  [League.Carbonite]: 1,
  [League.Bronzium]: 1,
  [League.Chromium]: 2,
  [League.Aurodium]: 2,
  [League.Kyber]: 2,
};

const zoneTemplate: readonly Omit<ZoneRequirement, 'slots'>[] = [
  { zoneId: 'character-front-top', label: 'Character Front Top', type: 'ground' },
  { zoneId: 'character-front-bottom', label: 'Character Front Bottom', type: 'ground' },
  { zoneId: 'character-back-top', label: 'Character Back Top', type: 'ground' },
  { zoneId: 'character-back-bottom', label: 'Character Back Bottom', type: 'ground' },
  { zoneId: 'fleet-front', label: 'Fleet Front', type: 'fleet' },
  { zoneId: 'fleet-back', label: 'Fleet Back', type: 'fleet' },
];

export function getPlannerRules(mode: GacMode, league: League): PlannerRules {
  const groundSlots = groundSlotsByLeague[league];
  const fleetSlots = fleetSlotsByLeague[league];

  return {
    mode,
    league,
    zones: zoneTemplate.map((zone): ZoneRequirement => ({
      ...zone,
      slots: zone.type === 'ground' ? groundSlots : fleetSlots,
    })),
  };
}
