import { GacMode } from './gac-mode.enum';
import { League } from './league.enum';

export type ZoneType = 'characters' | 'ships';

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

const zoneTemplate = [
  { zoneId: 'north', label: 'North', type: 'characters' },
  { zoneId: 'ships', label: 'Ships', type: 'ships' },
  { zoneId: 'south-front', label: 'South Front', type: 'characters' },
  { zoneId: 'south-back', label: 'South Back', type: 'characters' },
] as const satisfies readonly Omit<ZoneRequirement, 'slots'>[];

type ZoneId = (typeof zoneTemplate)[number]['zoneId'];
type SlotMap = Readonly<Record<ZoneId, number>>;
type LeagueSlotMap = Readonly<Record<League, SlotMap>>;
type ModeSlotMap = Readonly<Record<GacMode, LeagueSlotMap>>;

const slotMapByMode: ModeSlotMap = {
  [GacMode.FiveVFive]: {
    [League.Carbonite]: { north: 1, 'south-front': 1, 'south-back': 1, ships: 1 },
    [League.Bronzium]: { north: 2, 'south-front': 2, 'south-back': 1, ships: 1 },
    [League.Chromium]: { north: 3, 'south-front': 2, 'south-back': 2, ships: 2 },
    [League.Aurodium]: { north: 3, 'south-front': 3, 'south-back': 3, ships: 2 },
    [League.Kyber]: { north: 4, 'south-front': 4, 'south-back': 3, ships: 3 },
  },
  [GacMode.ThreeVThree]: {
    [League.Carbonite]: { north: 1, 'south-front': 1, 'south-back': 1, ships: 1 },
    [League.Bronzium]: { north: 2, 'south-front': 3, 'south-back': 2, ships: 1 },
    [League.Chromium]: { north: 3, 'south-front': 4, 'south-back': 3, ships: 2 },
    [League.Aurodium]: { north: 4, 'south-front': 5, 'south-back': 4, ships: 2 },
    [League.Kyber]: { north: 5, 'south-front': 5, 'south-back': 5, ships: 3 },
  },
};

export function getPlannerRules(mode: GacMode, league: League): PlannerRules {
  const slotMap = slotMapByMode[mode][league];

  return {
    mode,
    league,
    zones: zoneTemplate.map((zone): ZoneRequirement => ({
      ...zone,
      slots: slotMap[zone.zoneId],
    })),
  };
}
