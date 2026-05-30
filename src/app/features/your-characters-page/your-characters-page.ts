import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Character } from '../../apiModels/character';
import { EarnableLocation } from '../../apiModels/earnable-location';
import { FarmingStatus } from '../../apiModels/farming-status';
import { CharactersApiService } from '../../characters/characters-api.service';
import { EarnableLocationsPipe } from '../../earnables/earnable-locations.pipe';
import { ShardConverter } from '../../earnables/shard-converter';

interface YourCharacterRow {
  id: string;
  name: string;
  stars: number;
  currentShards: number;
  shardsRemaining: number;
  farmingStatusOrder: number;
  farmingStatus: string;
  locations: ReadonlyArray<EarnableLocation>;
}

const MaximumCharacterShards = 330;
const farmingStatusOrder = new Map<FarmingStatus, number>([
  [FarmingStatus.Active, 0],
  [FarmingStatus.Backlog, 1],
  [FarmingStatus.Done, 2],
]);

@Component({
  selector: 'app-your-characters-page',
  imports: [EarnableLocationsPipe],
  templateUrl: './your-characters-page.html',
  styleUrl: './your-characters-page.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class YourCharactersPage {
  private readonly charactersApiService = inject(CharactersApiService);
  private readonly shardConverter = inject(ShardConverter);
  protected readonly searchTerm = signal('');

  private readonly characters = toSignal(this.charactersApiService.getCharactersForUser(), {
    initialValue: [] as Character[],
  });

  protected readonly rows = computed<ReadonlyArray<YourCharacterRow>>(() =>
    this.characters()
      .map((character) => this.toRow(character))
      .sort(
        (left, right) =>
          left.farmingStatusOrder - right.farmingStatusOrder || left.name.localeCompare(right.name, 'en-US'),
      ),
  );

  protected readonly filteredRows = computed<ReadonlyArray<YourCharacterRow>>(() => {
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

  private toRow(character: Character): YourCharacterRow {
    const shards = character.shards?.shards ?? 0;
    const stars = this.shardConverter.convertToStars(shards);

    return {
      id: character.id,
      name: character.name,
      stars: stars.stars,
      currentShards: stars.shards,
      shardsRemaining: MaximumCharacterShards - shards,
      farmingStatusOrder: this.getFarmingStatusOrder(character.shards?.farmingStatus),
      farmingStatus: this.formatFarmingStatus(character.shards?.farmingStatus),
      locations: character.locations,
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
