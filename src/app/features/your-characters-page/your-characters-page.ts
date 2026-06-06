import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Character } from '../../apiModels/character';
import { CharactersApiService } from '../../characters/characters-api.service';
import { ShardConverter } from '../../earnables/shard-converter';
import { toSortedYourEarnableRows, YourEarnableRow } from '../your-earnables/your-earnable-row';
import { YourEarnablesTable } from '../your-earnables/your-earnables-table';

@Component({
  selector: 'app-your-characters-page',
  imports: [YourEarnablesTable],
  templateUrl: './your-characters-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class YourCharactersPage {
  private readonly charactersApiService = inject(CharactersApiService);
  private readonly shardConverter = inject(ShardConverter);

  private readonly characters = toSignal(this.charactersApiService.getCharactersForUser(), {
    initialValue: [] as Character[],
  });

  protected readonly rows = computed<ReadonlyArray<YourEarnableRow>>(() =>
    toSortedYourEarnableRows(this.characters(), this.shardConverter),
  );
}
