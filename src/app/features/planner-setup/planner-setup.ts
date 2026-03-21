import { ChangeDetectionStrategy, Component, computed, effect, signal } from '@angular/core';
import { GacMode } from '../../gac/gac-mode.enum';
import { League } from '../../gac/league.enum';
import { getPlannerRules } from '../../gac/planner-rules';
import { isGacMode, isLeague } from '../../gac/type-guards';

export const GAC_MODE_STORAGE_KEY = 'gac.mode';
export const LEAGUE_STORAGE_KEY = 'gac.league';

interface ZoneSlotState {
  teams: string[];
  defeated: boolean[];
  attackOptions: string[][];
}

type ZoneStateMap = Record<string, ZoneSlotState>;

const zoneUnlockDependency: Readonly<Record<string, string>> = {
  'south-back': 'south-front',
  ships: 'north',
};

@Component({
  selector: 'app-planner-setup',
  imports: [],
  templateUrl: './planner-setup.html',
  styleUrl: './planner-setup.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PlannerSetup {
  protected readonly gacModes = [GacMode.ThreeVThree, GacMode.FiveVFive] as const;
  protected readonly leagues = [
    League.Carbonite,
    League.Bronzium,
    League.Chromium,
    League.Aurodium,
    League.Kyber,
  ] as const;

  protected readonly selectedMode = signal<GacMode>(this.readStoredMode());
  protected readonly selectedLeague = signal<League>(this.readStoredLeague());
  protected readonly zoneState = signal<ZoneStateMap>({});
  protected readonly plannerRules = computed(() =>
    getPlannerRules(this.selectedMode(), this.selectedLeague()),
  );
  protected readonly zoneCards = computed(() =>
    this.plannerRules().zones.map((zone) => {
      const zoneState = this.zoneState()[zone.zoneId] ?? { teams: [], defeated: [], attackOptions: [] };
      const isUnlocked = this.isZoneUnlocked(zone.zoneId);
      const dependency = zoneUnlockDependency[zone.zoneId];

      return {
        ...zone,
        isUnlocked,
        unlockMessage:
          dependency && !isUnlocked
            ? `Locked until all ${this.formatZoneLabel(dependency)} teams are defeated.`
            : null,
        teamSlots: Array.from({ length: zone.slots }, (_, index) => ({
          teamName: zoneState.teams[index] ?? '',
          defeated: zoneState.defeated[index] ?? false,
          attackOptions: zoneState.attackOptions[index] ?? [],
          isEditable: isUnlocked && !(zoneState.defeated[index] ?? false),
        })),
      };
    }),
  );

  public constructor() {
    effect(() => {
      this.writeStorage(GAC_MODE_STORAGE_KEY, this.selectedMode());
      this.writeStorage(LEAGUE_STORAGE_KEY, this.selectedLeague());
    });

    effect(() => {
      const zones = this.plannerRules().zones;
      this.zoneState.update((previousState) => {
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
          };
        }

        return nextState;
      });
    });
  }

  protected onModeChanged(event: Event): void {
    const mode = (event.target as HTMLSelectElement | null)?.value;

    if (!mode || !isGacMode(mode)) {
      return;
    }

    this.selectedMode.set(mode);
  }

  protected onLeagueChanged(event: Event): void {
    const league = (event.target as HTMLSelectElement | null)?.value;

    if (!league || !isLeague(league)) {
      return;
    }

    this.selectedLeague.set(league);
  }

  protected formatLeague(league: League): string {
    return league.charAt(0).toUpperCase() + league.slice(1);
  }

  protected onDefenseTeamChanged(zoneId: string, slotIndex: number, event: Event): void {
    if (!this.isZoneUnlocked(zoneId) || this.isSlotDefeated(zoneId, slotIndex)) {
      return;
    }

    const teamName = (event.target as HTMLInputElement | null)?.value ?? '';
    this.zoneState.update((currentState) =>
      this.updateZoneSlotState(currentState, zoneId, slotIndex, { teamName }),
    );
  }

  protected onDefenseDefeatedChanged(zoneId: string, slotIndex: number, event: Event): void {
    if (!this.isZoneUnlocked(zoneId)) {
      return;
    }

    const defeated = (event.target as HTMLInputElement | null)?.checked ?? false;
    this.zoneState.update((currentState) =>
      this.updateZoneSlotState(currentState, zoneId, slotIndex, { defeated }),
    );
  }

  protected addSlotAttackOption(
    zoneId: string,
    slotIndex: number,
    inputElement: HTMLInputElement,
  ): void {
    if (!this.isZoneUnlocked(zoneId) || this.isSlotDefeated(zoneId, slotIndex)) {
      return;
    }

    const teamName = inputElement.value.trim();
    if (!teamName) {
      return;
    }

    this.zoneState.update((currentState) =>
      this.updateZoneSlotState(currentState, zoneId, slotIndex, {
        attackOptions: [...(currentState[zoneId]?.attackOptions[slotIndex] ?? []), teamName],
      }),
    );
    inputElement.value = '';
  }

  protected removeSlotAttackOption(zoneId: string, slotIndex: number, optionIndex: number): void {
    if (!this.isZoneUnlocked(zoneId) || this.isSlotDefeated(zoneId, slotIndex)) {
      return;
    }

    this.zoneState.update((currentState) =>
      this.updateZoneSlotState(currentState, zoneId, slotIndex, {
        attackOptions: (currentState[zoneId]?.attackOptions[slotIndex] ?? []).filter(
          (_, index) => index !== optionIndex,
        ),
      }),
    );
  }

  protected formatZoneLabel(zoneId: string): string {
    const zone = this.plannerRules().zones.find((entry) => entry.zoneId === zoneId);
    return zone ? zone.label : zoneId;
  }

  protected isZoneUnlocked(zoneId: string): boolean {
    const dependencyZoneId = zoneUnlockDependency[zoneId];
    if (!dependencyZoneId) {
      return true;
    }

    return this.areAllZoneTeamsDefeated(dependencyZoneId);
  }

  private areAllZoneTeamsDefeated(zoneId: string): boolean {
    const zone = this.zoneState()[zoneId];
    if (!zone || zone.defeated.length === 0) {
      return false;
    }

    return zone.defeated.every(Boolean);
  }

  private isSlotDefeated(zoneId: string, slotIndex: number): boolean {
    return this.zoneState()[zoneId]?.defeated[slotIndex] ?? false;
  }

  private updateZoneSlotState(
    currentState: ZoneStateMap,
    zoneId: string,
    slotIndex: number,
    changes: { teamName?: string; defeated?: boolean; attackOptions?: string[] },
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

    return {
      ...currentState,
      [zoneId]: {
        teams,
        defeated,
        attackOptions,
      },
    };
  }

  private readStoredMode(): GacMode {
    const storedMode = this.readStorage(GAC_MODE_STORAGE_KEY);
    return storedMode && isGacMode(storedMode) ? storedMode : GacMode.FiveVFive;
  }

  private readStoredLeague(): League {
    const storedLeague = this.readStorage(LEAGUE_STORAGE_KEY);
    return storedLeague && isLeague(storedLeague) ? storedLeague : League.Carbonite;
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
      // Ignore storage failures to keep the component usable.
    }
  }
}
