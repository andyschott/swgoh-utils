import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { RiseOfTheEmpire, RiseOfTheEmpireRewards } from '../../tb/rise-of-the-empire';

@Component({
  selector: 'app-rise-of-the-empire-rewards',
  imports: [RouterLink],
  templateUrl: './rise-of-the-empire-rewards.html',
  styleUrl: './rise-of-the-empire-rewards.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RiseOfTheEmpireRewardsPage {
  private readonly riseOfTheEmpire = inject(RiseOfTheEmpire);
  private readonly starLevels = Array.from({ length: 56 }, (_, index) => 56 - index);

  protected readonly rewards = computed<ReadonlyArray<RiseOfTheEmpireRewards>>(() =>
    this.starLevels
      .map((stars) => this.riseOfTheEmpire.getRewards(stars))
      .sort((left, right) => (right.stars ?? 0) - (left.stars ?? 0)),
  );

  protected formatNumber(value: number | null): string {
    return value === null ? '?' : value.toLocaleString('en-US');
  }

  protected formatRange(values: ReadonlyArray<number> | null): string {
    if (values === null) {
      return '?';
    }

    return values.join(' + ');
  }
}
