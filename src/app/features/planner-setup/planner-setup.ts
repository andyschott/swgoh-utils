import { ChangeDetectionStrategy, Component, computed, signal } from '@angular/core';
import { GacMode } from '../../gac/gac-mode.enum';
import { League } from '../../gac/league.enum';
import { getPlannerRules } from '../../gac/planner-rules';
import { isGacMode, isLeague } from '../../gac/type-guards';

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

  protected readonly selectedMode = signal<GacMode>(GacMode.FiveVFive);
  protected readonly selectedLeague = signal<League>(League.Carbonite);
  protected readonly plannerRules = computed(() =>
    getPlannerRules(this.selectedMode(), this.selectedLeague()),
  );

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
}
