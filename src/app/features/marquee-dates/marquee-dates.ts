import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MarqueeDate, MarqueeDates } from '../../marquee/marquee-dates';

interface MarqueeDateRow extends MarqueeDate {
  name: string;
}

@Component({
  selector: 'app-marquee-dates',
  imports: [RouterLink],
  templateUrl: './marquee-dates.html',
  styleUrl: './marquee-dates.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MarqueeDatesPage {
  private readonly marqueeDatesService = inject(MarqueeDates);
  protected readonly searchTerm = signal('');

  protected readonly marqueeDates = computed<ReadonlyArray<MarqueeDateRow>>(() =>
    this.marqueeDatesService
      .getNames()
      .map((name) => ({
        name,
        dates: this.marqueeDatesService.getDates(name),
      }))
      .filter((entry): entry is { name: string; dates: MarqueeDate } => entry.dates !== null)
      .map(({ name, dates }) => ({
        name,
        introduction: dates.introduction,
        marqueeEvent: dates.marqueeEvent,
        shipment: dates.shipment,
        farm: dates.farm,
        acceleration: dates.acceleration,
      }))
      .sort(
        (left, right) =>
          this.compareDatesDescending(left.introduction, right.introduction) ||
          this.compareDatesDescending(left.marqueeEvent, right.marqueeEvent),
      ),
  );

  protected readonly filteredMarqueeDates = computed<ReadonlyArray<MarqueeDateRow>>(() => {
    const normalizedSearch = this.searchTerm().trim().toLocaleLowerCase();

    if (!normalizedSearch) {
      return this.marqueeDates();
    }

    return this.marqueeDates().filter((marquee) =>
      marquee.name.toLocaleLowerCase().includes(normalizedSearch),
    );
  });

  protected onSearchInput(event: Event): void {
    const input = event.target as HTMLInputElement | null;
    this.searchTerm.set(input?.value ?? '');
  }

  protected formatDate(value: Date | null): string {
    if (value === null) {
      return '';
    }

    return value.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  }

  protected isFutureDate(value: Date | null): boolean {
    if (value === null) {
      return false;
    }

    return value.getTime() > Date.now();
  }

  private compareDatesDescending(left: Date | null, right: Date | null): number {
    if (left === null && right === null) {
      return 0;
    }

    if (left === null) {
      return 1;
    }

    if (right === null) {
      return -1;
    }

    return right.getTime() - left.getTime();
  }
}
