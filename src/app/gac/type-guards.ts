import { GacMode } from './gac-mode.enum';
import { League } from './league.enum';

const gacModeSet = new Set<string>(Object.values(GacMode));
const leagueSet = new Set<string>(Object.values(League));

export function isGacMode(value: string): value is GacMode {
  return gacModeSet.has(value);
}

export function isLeague(value: string): value is League {
  return leagueSet.has(value);
}
