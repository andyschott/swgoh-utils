import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Character } from '../../apiModels/character';
import { EarnableLocation } from '../../apiModels/earnable-location';
import { CharactersApiService } from '../../characters/characters-api.service';
import { EarnableLocationsPipe } from '../../earnables/earnable-locations.pipe';

interface CharacterRow {
  id: string;
  name: string;
  isAccelerated: boolean;
  locations: ReadonlyArray<EarnableLocation>;
}

@Component({
  selector: 'app-characters-page',
  imports: [EarnableLocationsPipe],
  templateUrl: './characters-page.html',
  styleUrl: './characters-page.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CharactersPage {
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
        locations: character.locations,
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
}
