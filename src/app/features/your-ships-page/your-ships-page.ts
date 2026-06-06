import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { EarnableLocation } from '../../apiModels/earnable-location';
import { FarmingStatus } from '../../apiModels/farming-status';
import { Ship } from '../../apiModels/ship';
import { EarnableLocationsPipe } from '../../earnables/earnable-locations.pipe';
import { ShardConverter } from '../../earnables/shard-converter';
import { ShipsApiService } from '../../ships/ships-api.service';

interface YourShipRow {
  id: string;
  name: string;
  stars: number;
  currentShards: number;
  shardsRemaining: number;
  farmingStatusOrder: number;
  farmingStatus: string;
  locations: ReadonlyArray<EarnableLocation>;
}

const MaximumShipShards = 330;
const farmingStatusOrder = new Map<FarmingStatus, number>([
  [FarmingStatus.Active, 0],
  [FarmingStatus.Backlog, 1],
  [FarmingStatus.Done, 2],
]);

@Component({
  selector: 'app-your-ships-page',
  imports: [EarnableLocationsPipe],
  templateUrl: './your-ships-page.html',
  styleUrl: './your-ships-page.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class YourShipsPage {
  private readonly shipsApiService = inject(ShipsApiService);
  private readonly shardConverter = inject(ShardConverter);
  protected readonly searchTerm = signal('');

  private readonly ships = toSignal(this.shipsApiService.getShipsForUser(), {
    initialValue: [] as Ship[],
  });

  protected readonly rows = computed<ReadonlyArray<YourShipRow>>(() =>
    this.ships()
      .map((ship) => this.toRow(ship))
      .sort(
        (left, right) =>
          left.farmingStatusOrder - right.farmingStatusOrder || left.name.localeCompare(right.name, 'en-US'),
      ),
  );

  protected readonly filteredRows = computed<ReadonlyArray<YourShipRow>>(() => {
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

  private toRow(ship: Ship): YourShipRow {
    const shards = ship.shards?.shards ?? 0;
    const stars = this.shardConverter.convertToStars(shards);

    return {
      id: ship.id,
      name: ship.name,
      stars: stars.stars,
      currentShards: stars.shards,
      shardsRemaining: MaximumShipShards - shards,
      farmingStatusOrder: this.getFarmingStatusOrder(ship.shards?.farmingStatus),
      farmingStatus: this.formatFarmingStatus(ship.shards?.farmingStatus),
      locations: ship.locations,
    };
  }

  private getFarmingStatusOrder(farmingStatus: FarmingStatus | null | undefined): number {
    return farmingStatusOrder.get(farmingStatus ?? FarmingStatus.Backlog) ?? farmingStatusOrder.get(FarmingStatus.Backlog) ?? 1;
  }

  private formatFarmingStatus(farmingStatus: FarmingStatus | null | undefined): string {
    switch (farmingStatus) {
      case FarmingStatus.Active:
        return 'Active';
      case FarmingStatus.Done:
        return 'Done';
      case FarmingStatus.Backlog:
        return 'Backlog';
      default:
        return 'Backlog';
    }
  }
}
