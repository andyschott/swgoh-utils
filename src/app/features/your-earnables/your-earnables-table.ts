import { ChangeDetectionStrategy, Component, computed, input, signal } from '@angular/core';
import { EarnableLocationsPipe } from '../../earnables/earnable-locations.pipe';
import { YourEarnableRow } from './your-earnable-row';

@Component({
  selector: 'app-your-earnables-table',
  imports: [EarnableLocationsPipe],
  templateUrl: './your-earnables-table.html',
  styleUrl: './your-earnables-table.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class YourEarnablesTable {
  public readonly title = input.required<string>();
  public readonly searchInputId = input.required<string>();
  public readonly searchAriaLabel = input.required<string>();
  public readonly searchPlaceholder = input.required<string>();
  public readonly caption = input.required<string>();
  public readonly nameColumnLabel = input.required<string>();
  public readonly emptyMessage = input.required<string>();
  public readonly rows = input.required<ReadonlyArray<YourEarnableRow>>();

  protected readonly searchTerm = signal('');

  protected readonly filteredRows = computed<ReadonlyArray<YourEarnableRow>>(() => {
    const normalizedSearch = this.searchTerm().trim().toLocaleLowerCase();

    if (!normalizedSearch) {
      return this.rows();
    }

    return this.rows().filter((row) => row.name.toLocaleLowerCase().includes(normalizedSearch));
  });

  protected onSearchInput(event: Event): void {
    const inputElement = event.target as HTMLInputElement | null;
    this.searchTerm.set(inputElement?.value ?? '');
  }
}
