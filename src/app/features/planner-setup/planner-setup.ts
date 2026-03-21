import { ChangeDetectionStrategy, Component, computed, effect, signal } from '@angular/core';
import { GacMode } from '../../gac/gac-mode.enum';
import { League } from '../../gac/league.enum';
import { getPlannerRules } from '../../gac/planner-rules';
import { isGacMode, isLeague } from '../../gac/type-guards';

export const GAC_MODE_STORAGE_KEY = 'gac.mode';
export const LEAGUE_STORAGE_KEY = 'gac.league';

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
  protected readonly plannerRules = computed(() =>
    getPlannerRules(this.selectedMode(), this.selectedLeague()),
  );

  public constructor() {
    effect(() => {
      this.writeStorage(GAC_MODE_STORAGE_KEY, this.selectedMode());
      this.writeStorage(LEAGUE_STORAGE_KEY, this.selectedLeague());
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
