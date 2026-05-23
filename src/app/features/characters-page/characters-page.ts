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
    ['0', 'Marquee Event'],
    ['MarqueeEvent', 'Marquee Event'],
    ['1', 'Crystal Shipments'],
    ['CrystalShipments', 'Crystal Shipments'],
    ['2', 'Dark Side'],
    ['DarkSide', 'Dark Side'],
    ['3', 'Light Side'],
    ['LightSide', 'Light Side'],
    ['4', 'Cantina'],
    ['Cantina', 'Cantina'],
    ['5', 'Fleet'],
    ['Fleet', 'Fleet'],
    ['6', 'Cantina Shipments'],
    ['CantinaShipments', 'Cantina Shipments'],
    ['7', 'Guild Token Shipments'],
    ['GuildTokenShipments', 'Guild Token Shipments'],
    ['8', 'Raid Mark 1 Shipments'],
    ['RaidMark1Shipments', 'Raid Mark 1 Shipments'],
    ['9', 'Raid Mark 2 Shipments'],
    ['RaidMark2Shipments', 'Raid Mark 2 Shipments'],
    ['10', 'Squad Arena Shipments'],
    ['SquadArenaShipments', 'Squad Arena Shipments'],
    ['11', 'Galactic War Shipments'],
    ['GalacticWarShipments', 'Galactic War Shipments'],
    ['12', 'Fleet Arena Shipments'],
    ['FleetArenaShipments', 'Fleet Arena Shipments'],
    ['13', 'Guild Event Mark 1 Shipments'],
    ['GuildEventMark1Shipments', 'Guild Event Mark 1 Shipments'],
    ['14', 'Guild Event Mark 2 Shipments'],
    ['GuildEventMark2Shipments', 'Guild Event Mark 2 Shipments'],
    ['15', 'Guild Event Mark 3 Shipments'],
    ['GuildEventMark3Shipments', 'Guild Event Mark 3 Shipments'],
    ['16', 'Shard Shop Currency'],
    ['ShardShopCurrency', 'Shard Shop Currency'],
    ['17', 'Legend Tokens'],
    ['LegendTokens', 'Legend Tokens'],
    ['18', 'Conquest Main Reward'],
    ['ConquestMainReward', 'Conquest Main Reward'],
    ['19', 'Conquest Secondary Reward'],
    ['ConquestSecondaryReward', 'Conquest Secondary Reward'],
    ['20', 'Conquest Shipments'],
    ['ConquestShipments', 'Conquest Shipments'],
    ['21', 'Proving Grounds'],
    ['ProvingGrounds', 'Proving Grounds'],
    ['22', 'Journey Guide'],
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
