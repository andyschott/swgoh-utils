import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { MarqueeDates } from '../../marquee/marquee-dates';
import { SignalDataFarming } from '../../signalData/signal-data-farming';

interface SignalDataFarmStorage {
  fragmentedCurrent: string;
  fragmentedTarget: string;
  incompleteCurrent: string;
  incompleteTarget: string;
  flawedCurrent: string;
  flawedTarget: string;
}

const SIGNAL_DATA_FARMING_STORAGE_KEY = 'signal-data-farming.inputs';

@Component({
  selector: 'app-home',
  templateUrl: './home.html',
  styleUrl: './home.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Home {
  private readonly marqueeDates = inject(MarqueeDates);
  private readonly signalDataFarming = inject(SignalDataFarming);

  protected readonly nextUpcomingMarquee = computed(() => {
    const now = Date.now();
    const upcoming = this.marqueeDates
      .getNames()
      .map((name) => this.marqueeDates.getDates(name))
      .filter((item): item is NonNullable<typeof item> => item !== null)
      .filter((item) => item.marqueeEvent.getTime() >= now)
      .sort((left, right) => left.marqueeEvent.getTime() - right.marqueeEvent.getTime());

    return upcoming[0] ?? null;
  });

  protected readonly signalDataRemaining = computed(() => {
    const inputs = this.readStoredInputs();
    return {
      fragmented: this.calculateRemaining(inputs.fragmentedCurrent, inputs.fragmentedTarget),
      incomplete: this.calculateRemaining(inputs.incompleteCurrent, inputs.incompleteTarget),
      flawed: this.calculateRemaining(inputs.flawedCurrent, inputs.flawedTarget),
    };
  });

  protected readonly hasRemainingSignalData = computed(() => {
    const remaining = this.signalDataRemaining();
    return remaining.fragmented > 0 || remaining.incomplete > 0 || remaining.flawed > 0;
  });

  protected readonly signalDataFarmingPlan = computed(() => {
    const remaining = this.signalDataRemaining();
    return this.signalDataFarming.getRemainingDays(
      remaining.fragmented,
      remaining.incomplete,
      remaining.flawed,
    );
  });

  protected formatDate(value: Date): string {
    return value.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  }

  private calculateRemaining(currentRaw: unknown, targetRaw: unknown): number {
    const current = this.parseNonNegativeNumber(currentRaw);
    const target = this.parseNonNegativeNumber(targetRaw);
    return Math.max(target - current, 0);
  }

  private parseNonNegativeNumber(rawValue: unknown): number {
    const asNumber = Number(rawValue);
    if (!Number.isFinite(asNumber) || asNumber < 0) {
      return 0;
    }

    return Math.floor(asNumber);
  }

  private readStoredInputs(): SignalDataFarmStorage {
    const defaultValues: SignalDataFarmStorage = {
      fragmentedCurrent: '0',
      fragmentedTarget: '0',
      incompleteCurrent: '0',
      incompleteTarget: '0',
      flawedCurrent: '0',
      flawedTarget: '0',
    };

    if (typeof localStorage === 'undefined') {
      return defaultValues;
    }

    try {
      const rawValue = localStorage.getItem(SIGNAL_DATA_FARMING_STORAGE_KEY);
      if (rawValue === null) {
        return defaultValues;
      }

      const parsedValue = JSON.parse(rawValue) as Partial<SignalDataFarmStorage>;
      return {
        fragmentedCurrent: this.readStringValue(parsedValue.fragmentedCurrent, defaultValues.fragmentedCurrent),
        fragmentedTarget: this.readStringValue(parsedValue.fragmentedTarget, defaultValues.fragmentedTarget),
        incompleteCurrent: this.readStringValue(parsedValue.incompleteCurrent, defaultValues.incompleteCurrent),
        incompleteTarget: this.readStringValue(parsedValue.incompleteTarget, defaultValues.incompleteTarget),
        flawedCurrent: this.readStringValue(parsedValue.flawedCurrent, defaultValues.flawedCurrent),
        flawedTarget: this.readStringValue(parsedValue.flawedTarget, defaultValues.flawedTarget),
      };
    } catch {
      return defaultValues;
    }
  }

  private readStringValue(value: unknown, fallback: string): string {
    return typeof value === 'string' ? value : fallback;
  }
}
