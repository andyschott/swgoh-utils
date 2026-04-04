import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { PlannerZoneCard } from './planner-zone-card';
import {
  ZoneCardViewModel,
  ZoneSlotDefeatedEvent,
  ZoneSlotDraftEvent,
  ZoneSlotEvent,
  ZoneSlotOptionEvent,
  ZoneSlotTeamEvent,
} from './planner-types';

@Component({
  selector: 'app-planner-defense-section',
  imports: [PlannerZoneCard],
  templateUrl: './planner-defense-section.html',
  styleUrl: './planner-defense-section.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PlannerDefenseSection {
  public readonly zoneCards = input.required<readonly ZoneCardViewModel[]>();
  public readonly zoneMiniNavCards = input.required<readonly ZoneCardViewModel[]>();

  public readonly clearRequested = output<void>();
  public readonly zoneScrollRequested = output<string>();
  public readonly zoneCollapsedToggleRequested = output<string>();
  public readonly teamExpandRequested = output<ZoneSlotEvent>();
  public readonly defenseTeamChanged = output<ZoneSlotTeamEvent>();
  public readonly defenseDefeatedChanged = output<ZoneSlotDefeatedEvent>();
  public readonly slotAttackOptionAddRequested = output<ZoneSlotEvent>();
  public readonly slotAttackOptionRemoveRequested = output<ZoneSlotOptionEvent>();
  public readonly slotAttackOptionDraftChanged = output<ZoneSlotDraftEvent>();

  protected onClearRequested(): void {
    this.clearRequested.emit();
  }

  protected onZoneScrollRequested(zoneId: string): void {
    this.zoneScrollRequested.emit(zoneId);
  }
}
