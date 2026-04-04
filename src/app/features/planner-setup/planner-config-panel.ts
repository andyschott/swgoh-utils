import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { GacMode } from '../../gac/gac-mode.enum';
import { League } from '../../gac/league.enum';
import { isGacMode, isLeague } from '../../gac/type-guards';

@Component({
  selector: 'app-planner-config-panel',
  imports: [],
  templateUrl: './planner-config-panel.html',
  styleUrl: './planner-config-panel.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PlannerConfigPanel {
  public readonly gacModes = input.required<readonly GacMode[]>();
  public readonly leagues = input.required<readonly League[]>();
  public readonly selectedMode = input.required<GacMode>();
  public readonly selectedLeague = input.required<League>();

  public readonly modeChanged = output<GacMode>();
  public readonly leagueChanged = output<League>();

  protected onModeChanged(event: Event): void {
    const mode = (event.target as HTMLSelectElement | null)?.value;
    if (!mode || !isGacMode(mode)) {
      return;
    }

    this.modeChanged.emit(mode);
  }

  protected onLeagueChanged(event: Event): void {
    const league = (event.target as HTMLSelectElement | null)?.value;
    if (!league || !isLeague(league)) {
      return;
    }

    this.leagueChanged.emit(league);
  }

  protected formatLeague(league: League): string {
    return league.charAt(0).toUpperCase() + league.slice(1);
  }
}
