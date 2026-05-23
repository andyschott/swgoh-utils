import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Character } from '../../apiModels/character';
import { EarnableLocation } from '../../apiModels/earnable-location';
import { CharactersApiService } from '../../characters/characters-api.service';

interface CharacterRow {
  id: string;
  name: string;
  isAccelerated: boolean;
  locations: string;
}

@Component({
  selector: 'app-characters-page',
  templateUrl: './characters-page.html',
  styleUrl: './characters-page.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CharactersPage {
  private static readonly locationNames = new Map<string, string>([
    ['MarqueeEvent', 'Marquee Event'],
    ['CrystalShipments', 'Crystal Shipments'],
    ['DarkSide', 'Dark Side'],
    ['LightSide', 'Light Side'],
    ['Cantina', 'Cantina'],
    ['Fleet', 'Fleet'],
    ['CantinaShipments', 'Cantina Shipments'],
    ['GuildTokenShipments', 'Guild Token Shipments'],
    ['RaidMark1Shipments', 'Raid Mark 1 Shipments'],
    ['RaidMark2Shipments', 'Raid Mark 2 Shipments'],
    ['SquadArenaShipments', 'Squad Arena Shipments'],
    ['GalacticWarShipments', 'Galactic War Shipments'],
    ['FleetArenaShipments', 'Fleet Arena Shipments'],
    ['GuildEventMark1Shipments', 'Guild Event Mark 1 Shipments'],
    ['GuildEventMark2Shipments', 'Guild Event Mark 2 Shipments'],
    ['GuildEventMark3Shipments', 'Guild Event Mark 3 Shipments'],
    ['ShardShopCurrency', 'Shard Shop Currency'],
    ['LegendTokens', 'Legend Tokens'],
    ['ConquestMainReward', 'Conquest Main Reward'],
    ['ConquestSecondaryReward', 'Conquest Secondary Reward'],
    ['ConquestShipments', 'Conquest Shipments'],
    ['ProvingGrounds', 'Proving Grounds'],
    ['JourneyGuide', 'Journey Guide'],
  ]);

  private readonly charactersApiService = inject(CharactersApiService);
  protected readonly searchTerm = signal('');

  private readonly characters = toSignal(this.charactersApiService.getCharacters(), {
    initialValue: [] as Character[],
  });

  protected readonly rows = computed<ReadonlyArray<CharacterRow>>(() =>
    this.characters()
      .map((character) => ({
        id: character.id,
        name: character.name,
        isAccelerated: character.isAccelerated,
        locations: this.formatLocations(character.locations),
      }))
      .sort((left, right) => left.name.localeCompare(right.name, 'en-US')),
  );

  protected readonly filteredRows = computed<ReadonlyArray<CharacterRow>>(() => {
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

  private formatLocations(locations: ReadonlyArray<EarnableLocation>): string {
    if (locations.length === 0) {
      return 'None';
    }

    return locations
      .map((location) => this.formatLocation(location))
      .sort((left, right) => left.localeCompare(right, 'en-US'))
      .join(', ');
  }

  private formatLocation(location: EarnableLocation): string {
    const normalized = String(location);
    return CharactersPage.locationNames.get(normalized) ?? 'Unknown';
  }
}
