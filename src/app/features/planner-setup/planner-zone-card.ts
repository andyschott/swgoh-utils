import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { PlannerTeamSlot } from './planner-team-slot';
import {
  ZoneCardViewModel,
  ZoneSlotDefeatedEvent,
  ZoneSlotDraftEvent,
  ZoneSlotEvent,
  ZoneSlotOptionEvent,
  ZoneSlotTeamEvent,
} from './planner-types';

@Component({
  selector: 'app-planner-zone-card',
  imports: [PlannerTeamSlot],
  templateUrl: './planner-zone-card.html',
  styleUrl: './planner-zone-card.css',
  host: {
    '[attr.data-zone-id]': 'zone().zoneId',
  },
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PlannerZoneCard {
  public readonly zone = input.required<ZoneCardViewModel>();

  public readonly zoneCollapsedToggleRequested = output<string>();
  public readonly teamExpandRequested = output<ZoneSlotEvent>();
  public readonly defenseTeamChanged = output<ZoneSlotTeamEvent>();
  public readonly defenseDefeatedChanged = output<ZoneSlotDefeatedEvent>();
  public readonly slotAttackOptionAddRequested = output<ZoneSlotEvent>();
  public readonly slotAttackOptionRemoveRequested = output<ZoneSlotOptionEvent>();
  public readonly slotAttackOptionDraftChanged = output<ZoneSlotDraftEvent>();

  protected onZoneCollapsedToggleRequested(): void {
    this.zoneCollapsedToggleRequested.emit(this.zone().zoneId);
  }
}
