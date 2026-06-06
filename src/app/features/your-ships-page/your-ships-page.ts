import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Ship } from '../../apiModels/ship';
import { ShardConverter } from '../../earnables/shard-converter';
import { ShipsApiService } from '../../ships/ships-api.service';
import { toSortedYourEarnableRows, YourEarnableRow } from '../your-earnables/your-earnable-row';
import { YourEarnablesTable } from '../your-earnables/your-earnables-table';

@Component({
  selector: 'app-your-ships-page',
  imports: [YourEarnablesTable],
  templateUrl: './your-ships-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class YourShipsPage {
  private readonly shipsApiService = inject(ShipsApiService);
  private readonly shardConverter = inject(ShardConverter);

  private readonly ships = toSignal(this.shipsApiService.getShipsForUser(), {
    initialValue: [] as Ship[],
  });

  protected readonly rows = computed<ReadonlyArray<YourEarnableRow>>(() =>
    toSortedYourEarnableRows(this.ships(), this.shardConverter),
  );
}
