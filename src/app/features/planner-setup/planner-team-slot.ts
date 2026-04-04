import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { PlannerAttackOptions } from './planner-attack-options';
import {
  TeamSlotViewModel,
  ZoneSlotDefeatedEvent,
  ZoneSlotDraftEvent,
  ZoneSlotEvent,
  ZoneSlotOptionEvent,
  ZoneSlotTeamEvent,
} from './planner-types';

@Component({
  selector: 'app-planner-team-slot',
  imports: [PlannerAttackOptions],
  templateUrl: './planner-team-slot.html',
  styleUrl: './planner-team-slot.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PlannerTeamSlot {
  public readonly zoneId = input.required<string>();
  public readonly zoneLabel = input.required<string>();
  public readonly slotIndex = input.required<number>();
  public readonly slot = input.required<TeamSlotViewModel>();
  public readonly zoneUnlocked = input.required<boolean>();

  public readonly expandRequested = output<ZoneSlotEvent>();
  public readonly defenseDefeatedChanged = output<ZoneSlotDefeatedEvent>();
  public readonly defenseTeamChanged = output<ZoneSlotTeamEvent>();
  public readonly slotAttackOptionAddRequested = output<ZoneSlotEvent>();
  public readonly slotAttackOptionRemoveRequested = output<ZoneSlotOptionEvent>();
  public readonly slotAttackOptionDraftChanged = output<ZoneSlotDraftEvent>();

  protected onExpandRequested(): void {
    this.expandRequested.emit({ zoneId: this.zoneId(), slotIndex: this.slotIndex() });
  }

  protected onDefenseDefeatedChanged(event: Event): void {
    const defeated = (event.target as HTMLInputElement | null)?.checked ?? false;
    this.defenseDefeatedChanged.emit({
      zoneId: this.zoneId(),
      slotIndex: this.slotIndex(),
      defeated,
    });
  }

  protected onDefenseTeamChanged(event: Event): void {
    const teamName = (event.target as HTMLInputElement | null)?.value ?? '';
    this.defenseTeamChanged.emit({
      zoneId: this.zoneId(),
      slotIndex: this.slotIndex(),
      teamName,
    });
  }

  protected onSlotAttackOptionDraftChanged(draft: string): void {
    this.slotAttackOptionDraftChanged.emit({
      zoneId: this.zoneId(),
      slotIndex: this.slotIndex(),
      draft,
    });
  }

  protected onSlotAttackOptionAddRequested(): void {
    this.slotAttackOptionAddRequested.emit({ zoneId: this.zoneId(), slotIndex: this.slotIndex() });
  }

  protected onSlotAttackOptionRemoveRequested(optionIndex: number): void {
    this.slotAttackOptionRemoveRequested.emit({
      zoneId: this.zoneId(),
      slotIndex: this.slotIndex(),
      optionIndex,
    });
  }
}
