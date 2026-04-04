import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';

@Component({
  selector: 'app-planner-attack-options',
  imports: [],
  templateUrl: './planner-attack-options.html',
  styleUrl: './planner-attack-options.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PlannerAttackOptions {
  public readonly zoneId = input.required<string>();
  public readonly slotIndex = input.required<number>();
  public readonly draft = input.required<string>();
  public readonly options = input.required<readonly string[]>();
  public readonly editable = input.required<boolean>();

  public readonly draftChanged = output<string>();
  public readonly addRequested = output<void>();
  public readonly removeRequested = output<number>();

  protected onDraftChanged(event: Event): void {
    const value = (event.target as HTMLInputElement | null)?.value ?? '';
    this.draftChanged.emit(value);
  }

  protected onAddRequested(): void {
    this.addRequested.emit();
  }

  protected onRemoveRequested(optionIndex: number): void {
    this.removeRequested.emit(optionIndex);
  }
}
