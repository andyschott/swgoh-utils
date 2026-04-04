import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { GacMode } from '../../gac/gac-mode.enum';
import { League } from '../../gac/league.enum';
import { PlannerConfigPanel } from './planner-config-panel';
import { PlannerDefenseSection } from './planner-defense-section';
import { PlannerStateFacade } from './planner-state.facade';

export {
  DEFENSE_PLAN_STORAGE_KEY,
  DEFENSE_PLAN_UPDATED_AT_STORAGE_KEY,
  GAC_MODE_STORAGE_KEY,
  LEAGUE_STORAGE_KEY,
} from './planner-storage.constants';

@Component({
  selector: 'app-planner-setup',
  imports: [PlannerConfigPanel, PlannerDefenseSection],
  templateUrl: './planner-setup.html',
  styleUrl: './planner-setup.css',
  host: {
    '[attr.data-theme]': 'themeMode()',
  },
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PlannerSetup {
  private readonly plannerStateFacade = inject(PlannerStateFacade);

  protected readonly gacModes = this.plannerStateFacade.gacModes;
  protected readonly leagues = this.plannerStateFacade.leagues;
  protected readonly selectedMode = this.plannerStateFacade.selectedMode;
  protected readonly selectedLeague = this.plannerStateFacade.selectedLeague;
  protected readonly zoneCards = this.plannerStateFacade.zoneCards;
  protected readonly zoneMiniNavCards = this.plannerStateFacade.zoneMiniNavCards;
  protected readonly plannerRules = this.plannerStateFacade.plannerRules;
  protected readonly zoneState = this.plannerStateFacade.zoneState;
  protected readonly themeMode = this.plannerStateFacade.themeMode;

  protected onModeSelected(mode: GacMode): void {
    this.plannerStateFacade.setMode(mode);
  }

  protected onLeagueSelected(league: League): void {
    this.plannerStateFacade.setLeague(league);
  }

  protected expandCollapsedTeam(zoneId: string, slotIndex: number): void {
    this.plannerStateFacade.expandCollapsedTeam(zoneId, slotIndex);
  }

  protected addSlotAttackOption(zoneId: string, slotIndex: number): void {
    this.plannerStateFacade.addSlotAttackOption(zoneId, slotIndex);
  }

  protected removeSlotAttackOption(zoneId: string, slotIndex: number, optionIndex: number): void {
    this.plannerStateFacade.removeSlotAttackOption(zoneId, slotIndex, optionIndex);
  }

  protected clearDefenseAndAttackOptions(): void {
    this.plannerStateFacade.clearDefenseAndAttackOptions();
  }

  protected toggleZoneCollapsed(zoneId: string): void {
    this.plannerStateFacade.toggleZoneCollapsed(zoneId);
  }

  protected scrollToZone(zoneId: string): void {
    this.plannerStateFacade.scrollToZone(zoneId);
  }

  protected onDefenseTeamValueChanged(event: {
    zoneId: string;
    slotIndex: number;
    teamName: string;
  }): void {
    this.plannerStateFacade.setDefenseTeam(event.zoneId, event.slotIndex, event.teamName);
  }

  protected onDefenseDefeatedValueChanged(event: {
    zoneId: string;
    slotIndex: number;
    defeated: boolean;
  }): void {
    this.plannerStateFacade.setDefenseDefeated(event.zoneId, event.slotIndex, event.defeated);
  }

  protected onSlotAttackOptionDraftValueChanged(event: {
    zoneId: string;
    slotIndex: number;
    draft: string;
  }): void {
    this.plannerStateFacade.setSlotAttackOptionDraft(event.zoneId, event.slotIndex, event.draft);
  }
}
