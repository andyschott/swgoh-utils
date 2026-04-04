import { DestroyRef, Injectable, computed, effect, inject, signal } from '@angular/core';
import { GacMode } from '../../gac/gac-mode.enum';
import { League } from '../../gac/league.enum';
import { getPlannerRules } from '../../gac/planner-rules';
import { isGacMode, isLeague } from '../../gac/type-guards';
import {
  areAllZoneTeamsDefeated,
  buildEmptyZoneState,
  isSlotDefeated,
  syncZoneCollapsedStateWithRules,
  syncZoneStateWithRules,
  updateZoneSlotState,
  zoneUnlockDependency,
} from './planner-state.utils';
import { ThemeMode, ZoneCardViewModel, ZoneCollapseStateMap, ZoneStateMap } from './planner-types';
import {
  DEFENSE_PLAN_STORAGE_KEY,
  DEFENSE_PLAN_TTL_MS,
  DEFENSE_PLAN_UPDATED_AT_STORAGE_KEY,
  GAC_MODE_STORAGE_KEY,
  LEAGUE_STORAGE_KEY,
} from './planner-storage.constants';

@Injectable({ providedIn: 'root' })
export class PlannerStateFacade {
  private readonly destroyRef = inject(DestroyRef);

  public readonly gacModes = [GacMode.ThreeVThree, GacMode.FiveVFive] as const;
  public readonly leagues = [
    League.Carbonite,
    League.Bronzium,
    League.Chromium,
    League.Aurodium,
    League.Kyber,
  ] as const;

  public readonly selectedMode = signal<GacMode>(this.readStoredMode());
  public readonly selectedLeague = signal<League>(this.readStoredLeague());
  public readonly zoneState = signal<ZoneStateMap>(this.readStoredZoneState());
  public readonly zoneCollapsedState = signal<ZoneCollapseStateMap>({});
  public readonly themeMode = signal<ThemeMode>(this.getInitialThemeMode());
  public readonly plannerRules = computed(() =>
    getPlannerRules(this.selectedMode(), this.selectedLeague()),
  );
  public readonly zoneCards = computed<ZoneCardViewModel[]>(() =>
    this.plannerRules().zones.map((zone) => {
      const zoneState = this.zoneState()[zone.zoneId] ?? {
        teams: [],
        defeated: [],
        attackOptions: [],
        attackOptionDrafts: [],
      };
      const zoneUnlocked = this.isZoneUnlocked(zone.zoneId);
      const dependency = zoneUnlockDependency[zone.zoneId];

      return {
        ...zone,
        isUnlocked: zoneUnlocked,
        isCollapsed: this.zoneCollapsedState()[zone.zoneId] ?? false,
        unlockMessage:
          dependency && !zoneUnlocked
            ? `Locked until all ${this.formatZoneLabel(dependency)} teams are defeated.`
            : null,
        teamSlots: Array.from({ length: zone.slots }, (_, index) => ({
          teamName: zoneState.teams[index] ?? '',
          defeated: zoneState.defeated[index] ?? false,
          attackOptions: zoneState.attackOptions[index] ?? [],
          attackOptionDraft: zoneState.attackOptionDrafts[index] ?? '',
          isEditable: zoneUnlocked && !(zoneState.defeated[index] ?? false),
        })),
      };
    }),
  );
  public readonly zoneMiniNavCards = computed(() => {
    const cards = this.zoneCards();
    const nonShips = cards.filter((zone) => zone.zoneId !== 'ships');
    const ships = cards.find((zone) => zone.zoneId === 'ships');
    return ships ? [...nonShips, ships] : nonShips;
  });

  public constructor() {
    this.initializeThemeSync();

    effect(() => {
      this.writeStorage(GAC_MODE_STORAGE_KEY, this.selectedMode());
      this.writeStorage(LEAGUE_STORAGE_KEY, this.selectedLeague());
    });

    effect(() => {
      this.writeStorage(DEFENSE_PLAN_STORAGE_KEY, JSON.stringify(this.zoneState()));
      this.writeStorage(DEFENSE_PLAN_UPDATED_AT_STORAGE_KEY, String(Date.now()));
    });

    effect(() => {
      const zones = this.plannerRules().zones;
      this.zoneState.update((previousState) => syncZoneStateWithRules(previousState, zones));
    });

    effect(() => {
      const zones = this.plannerRules().zones;
      this.zoneCollapsedState.update((previousState) =>
        syncZoneCollapsedStateWithRules(previousState, zones),
      );
    });
  }

  public setMode(mode: GacMode): void {
    this.selectedMode.set(mode);
  }

  public setLeague(league: League): void {
    this.selectedLeague.set(league);
  }

  public formatLeague(league: League): string {
    return league.charAt(0).toUpperCase() + league.slice(1);
  }

  public setDefenseTeam(zoneId: string, slotIndex: number, teamName: string): void {
    if (!this.isZoneUnlocked(zoneId) || isSlotDefeated(this.zoneState(), zoneId, slotIndex)) {
      return;
    }

    this.zoneState.update((currentState) =>
      updateZoneSlotState(currentState, zoneId, slotIndex, { teamName }),
    );
  }

  public setDefenseDefeated(zoneId: string, slotIndex: number, defeated: boolean): void {
    if (!this.isZoneUnlocked(zoneId)) {
      return;
    }

    this.zoneState.update((currentState) =>
      updateZoneSlotState(currentState, zoneId, slotIndex, { defeated }),
    );
  }

  public expandCollapsedTeam(zoneId: string, slotIndex: number): void {
    if (!this.isZoneUnlocked(zoneId)) {
      return;
    }

    this.zoneState.update((currentState) =>
      updateZoneSlotState(currentState, zoneId, slotIndex, { defeated: false }),
    );
  }

  public addSlotAttackOption(zoneId: string, slotIndex: number): void {
    if (!this.isZoneUnlocked(zoneId) || isSlotDefeated(this.zoneState(), zoneId, slotIndex)) {
      return;
    }

    const teamName = this.zoneState()[zoneId]?.attackOptionDrafts[slotIndex]?.trim() ?? '';
    if (!teamName) {
      return;
    }

    this.zoneState.update((currentState) =>
      updateZoneSlotState(currentState, zoneId, slotIndex, {
        attackOptions: [...(currentState[zoneId]?.attackOptions[slotIndex] ?? []), teamName],
        attackOptionDraft: '',
      }),
    );
  }

  public removeSlotAttackOption(zoneId: string, slotIndex: number, optionIndex: number): void {
    if (!this.isZoneUnlocked(zoneId) || isSlotDefeated(this.zoneState(), zoneId, slotIndex)) {
      return;
    }

    this.zoneState.update((currentState) =>
      updateZoneSlotState(currentState, zoneId, slotIndex, {
        attackOptions: (currentState[zoneId]?.attackOptions[slotIndex] ?? []).filter(
          (_, index) => index !== optionIndex,
        ),
      }),
    );
  }

  public setSlotAttackOptionDraft(zoneId: string, slotIndex: number, draft: string): void {
    if (!this.isZoneUnlocked(zoneId) || isSlotDefeated(this.zoneState(), zoneId, slotIndex)) {
      return;
    }

    this.zoneState.update((currentState) =>
      updateZoneSlotState(currentState, zoneId, slotIndex, { attackOptionDraft: draft }),
    );
  }

  public formatZoneLabel(zoneId: string): string {
    const zone = this.plannerRules().zones.find((entry) => entry.zoneId === zoneId);
    return zone ? zone.label : zoneId;
  }

  public isZoneUnlocked(zoneId: string): boolean {
    const dependencyZoneId = zoneUnlockDependency[zoneId];
    if (!dependencyZoneId) {
      return true;
    }

    return areAllZoneTeamsDefeated(this.zoneState(), dependencyZoneId);
  }

  public clearDefenseAndAttackOptions(): void {
    this.zoneState.set(buildEmptyZoneState(this.plannerRules().zones));
    this.removeStorage(DEFENSE_PLAN_STORAGE_KEY);
    this.removeStorage(DEFENSE_PLAN_UPDATED_AT_STORAGE_KEY);
  }

  public toggleZoneCollapsed(zoneId: string): void {
    this.zoneCollapsedState.update((currentState) => ({
      ...currentState,
      [zoneId]: !(currentState[zoneId] ?? false),
    }));
  }

  public scrollToZone(zoneId: string): void {
    const zoneElement = globalThis.document?.getElementById(`zone-card-${zoneId}`);
    if (!zoneElement) {
      return;
    }

    zoneElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }

  private readStoredMode(): GacMode {
    const storedMode = this.readStorage(GAC_MODE_STORAGE_KEY);
    return storedMode && isGacMode(storedMode) ? storedMode : GacMode.FiveVFive;
  }

  private readStoredLeague(): League {
    const storedLeague = this.readStorage(LEAGUE_STORAGE_KEY);
    return storedLeague && isLeague(storedLeague) ? storedLeague : League.Carbonite;
  }

  private readStoredZoneState(): ZoneStateMap {
    const storedPlan = this.readStorage(DEFENSE_PLAN_STORAGE_KEY);
    if (!storedPlan) {
      return {};
    }

    const storedUpdatedAt = this.readStorage(DEFENSE_PLAN_UPDATED_AT_STORAGE_KEY);
    if (storedUpdatedAt) {
      const updatedAtMs = Number(storedUpdatedAt);
      if (!Number.isFinite(updatedAtMs) || this.isDefensePlanExpired(updatedAtMs)) {
        this.removeStorage(DEFENSE_PLAN_STORAGE_KEY);
        this.removeStorage(DEFENSE_PLAN_UPDATED_AT_STORAGE_KEY);
        return {};
      }
    } else {
      // Legacy support for existing stored plans that predate timestamp tracking.
      this.writeStorage(DEFENSE_PLAN_UPDATED_AT_STORAGE_KEY, String(Date.now()));
    }

    try {
      const parsed = JSON.parse(storedPlan);
      if (!parsed || typeof parsed !== 'object') {
        return {};
      }

      const state: ZoneStateMap = {};
      for (const [zoneId, rawZone] of Object.entries(parsed)) {
        const zone = rawZone as {
          teams?: unknown;
          defeated?: unknown;
          attackOptions?: unknown;
          attackOptionDrafts?: unknown;
        };

        state[zoneId] = {
          teams: Array.isArray(zone.teams)
            ? zone.teams.map((team) => (typeof team === 'string' ? team : ''))
            : [],
          defeated: Array.isArray(zone.defeated)
            ? zone.defeated.map((value) => Boolean(value))
            : [],
          attackOptions: Array.isArray(zone.attackOptions)
            ? zone.attackOptions.map((options) =>
                Array.isArray(options)
                  ? options
                      .filter((value): value is string => typeof value === 'string')
                      .map((value) => value)
                  : [],
              )
            : [],
          attackOptionDrafts: Array.isArray(zone.attackOptionDrafts)
            ? zone.attackOptionDrafts.map((draft) => (typeof draft === 'string' ? draft : ''))
            : [],
        };
      }

      return state;
    } catch {
      return {};
    }
  }

  private readStorage(key: string): string | null {
    try {
      const storage = globalThis.localStorage as Partial<Storage> | undefined;
      return typeof storage?.getItem === 'function' ? storage.getItem(key) : null;
    } catch {
      return null;
    }
  }

  private writeStorage(key: string, value: string): void {
    try {
      const storage = globalThis.localStorage as Partial<Storage> | undefined;
      if (typeof storage?.setItem === 'function') {
        storage.setItem(key, value);
      }
    } catch {
      // Ignore storage failures to keep the planner usable.
    }
  }

  private removeStorage(key: string): void {
    try {
      const storage = globalThis.localStorage as Partial<Storage> | undefined;
      if (typeof storage?.removeItem === 'function') {
        storage.removeItem(key);
      }
    } catch {
      // Ignore storage failures to keep the planner usable.
    }
  }

  private isDefensePlanExpired(updatedAtMs: number): boolean {
    return Date.now() - updatedAtMs >= DEFENSE_PLAN_TTL_MS;
  }

  private initializeThemeSync(): void {
    const mediaQuery = this.getColorSchemeQuery();
    if (!mediaQuery) {
      return;
    }

    const applyTheme = (matchesDark: boolean): void => {
      this.themeMode.set(matchesDark ? 'dark' : 'light');
    };
    const onChange = (event: MediaQueryListEvent): void => {
      applyTheme(event.matches);
    };

    applyTheme(mediaQuery.matches);

    if (typeof mediaQuery.addEventListener === 'function') {
      mediaQuery.addEventListener('change', onChange);
      this.destroyRef.onDestroy(() => mediaQuery.removeEventListener('change', onChange));
      return;
    }

    if (typeof mediaQuery.addListener === 'function') {
      mediaQuery.addListener(onChange);
      this.destroyRef.onDestroy(() => mediaQuery.removeListener(onChange));
    }
  }

  private getInitialThemeMode(): ThemeMode {
    const mediaQuery = this.getColorSchemeQuery();
    return mediaQuery?.matches ? 'dark' : 'light';
  }

  private getColorSchemeQuery(): MediaQueryList | null {
    try {
      if (typeof globalThis.matchMedia !== 'function') {
        return null;
      }

      return globalThis.matchMedia('(prefers-color-scheme: dark)');
    } catch {
      return null;
    }
  }
}
