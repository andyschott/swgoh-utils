import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { EarnableLocation } from '../../apiModels/earnable-location';
import { Ship } from '../../apiModels/ship';
import { EarnableLocationsPipe } from '../../earnables/earnable-locations.pipe';
import { ShipsApiService } from '../../ships/ships-api.service';

interface ShipRow {
  id: string;
  name: string;
  locations: ReadonlyArray<EarnableLocation>;
}

@Component({
  selector: 'app-ships-page',
  imports: [EarnableLocationsPipe],
  templateUrl: './ships-page.html',
  styleUrl: './ships-page.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ShipsPage {
  private readonly shipsApiService = inject(ShipsApiService);
  protected readonly searchTerm = signal('');

  private readonly ships = toSignal(this.shipsApiService.getShips(), {
    initialValue: [] as Ship[],
  });

  protected readonly rows = computed<ReadonlyArray<ShipRow>>(() =>
    this.ships()
      .map((ship) => ({
        id: ship.id,
        name: ship.name,
        locations: ship.locations,
      }))
      .sort((left, right) => left.name.localeCompare(right.name, 'en-US')),
  );

  protected readonly filteredRows = computed<ReadonlyArray<ShipRow>>(() => {
    const normalizedSearch = this.searchTerm().trim().toLocaleLowerCase();

    if (!normalizedSearch) {
      return this.rows();
    }

    return this.rows().filter((row) => row.name.toLocaleLowerCase().includes(normalizedSearch));
  });

  protected onSearchInput(event: Event): void {
    const input = event.target as HTMLInputElement | null;
    this.searchTerm.set(input?.value ?? '');
  }
}
