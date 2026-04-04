import { GacMode } from '../../gac/gac-mode.enum';
import { League } from '../../gac/league.enum';

export type ThemeMode = 'light' | 'dark';

export interface ZoneSlotState {
  teams: string[];
  defeated: boolean[];
  attackOptions: string[][];
  attackOptionDrafts: string[];
}

export type ZoneStateMap = Record<string, ZoneSlotState>;
export type ZoneCollapseStateMap = Record<string, boolean>;

export interface TeamSlotViewModel {
  teamName: string;
  defeated: boolean;
  attackOptions: string[];
  attackOptionDraft: string;
  isEditable: boolean;
}

export interface ZoneCardViewModel {
  zoneId: string;
  label: string;
  slots: number;
  type: string;
  isUnlocked: boolean;
  isCollapsed: boolean;
  unlockMessage: string | null;
  teamSlots: TeamSlotViewModel[];
}

export interface ModeChangedEvent {
  mode: GacMode;
}

export interface LeagueChangedEvent {
  league: League;
}

export interface ZoneSlotEvent {
  zoneId: string;
  slotIndex: number;
}

export interface ZoneSlotOptionEvent extends ZoneSlotEvent {
  optionIndex: number;
}

export interface ZoneSlotTeamEvent extends ZoneSlotEvent {
  teamName: string;
}

export interface ZoneSlotDefeatedEvent extends ZoneSlotEvent {
  defeated: boolean;
}

export interface ZoneSlotDraftEvent extends ZoneSlotEvent {
  draft: string;
}

export interface ZoneSlotStateChanges {
  teamName?: string;
  defeated?: boolean;
  attackOptions?: string[];
  attackOptionDraft?: string;
}

export interface PlannerRuleZone {
  zoneId: string;
  label: string;
  slots: number;
  type: string;
}
