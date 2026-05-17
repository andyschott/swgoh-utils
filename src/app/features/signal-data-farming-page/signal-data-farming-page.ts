import { ChangeDetectionStrategy, Component, computed, effect, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { startWith } from 'rxjs';
import { SignalDataFarming } from '../../signalData/signal-data-farming';

interface SignalDataFarmForm {
  fragmentedCurrent: FormControl<string>;
  fragmentedTarget: FormControl<string>;
  incompleteCurrent: FormControl<string>;
  incompleteTarget: FormControl<string>;
  flawedCurrent: FormControl<string>;
  flawedTarget: FormControl<string>;
}

type SignalDataType = 'fragmented' | 'incomplete' | 'flawed';
interface SignalDataFarmStorage {
  fragmentedCurrent: string;
  fragmentedTarget: string;
  incompleteCurrent: string;
  incompleteTarget: string;
  flawedCurrent: string;
  flawedTarget: string;
}

interface SignalDataRow {
  key: SignalDataType;
  label: string;
  currentControlName: keyof SignalDataFarmForm;
  targetControlName: keyof SignalDataFarmForm;
}

const signalDataRows: ReadonlyArray<SignalDataRow> = [
  {
    key: 'fragmented',
    label: 'Fragmented Signal Data',
    currentControlName: 'fragmentedCurrent',
    targetControlName: 'fragmentedTarget',
  },
  {
    key: 'incomplete',
    label: 'Incomplete Signal Data',
    currentControlName: 'incompleteCurrent',
    targetControlName: 'incompleteTarget',
  },
  {
    key: 'flawed',
    label: 'Flawed Signal Data',
    currentControlName: 'flawedCurrent',
    targetControlName: 'flawedTarget',
  },
];
const SIGNAL_DATA_FARMING_STORAGE_KEY = 'signal-data-farming.inputs';

@Component({
  selector: 'app-signal-data-farming-page',
  imports: [ReactiveFormsModule],
  templateUrl: './signal-data-farming-page.html',
  styleUrl: './signal-data-farming-page.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SignalDataFarmingPage {
  private readonly signalDataFarming = inject(SignalDataFarming);
  private readonly storedInputs = this.readStoredInputs();
  protected readonly rows = signalDataRows;

  protected readonly form = new FormGroup<SignalDataFarmForm>({
    fragmentedCurrent: new FormControl(this.storedInputs.fragmentedCurrent, { nonNullable: true }),
    fragmentedTarget: new FormControl(this.storedInputs.fragmentedTarget, { nonNullable: true }),
    incompleteCurrent: new FormControl(this.storedInputs.incompleteCurrent, { nonNullable: true }),
    incompleteTarget: new FormControl(this.storedInputs.incompleteTarget, { nonNullable: true }),
    flawedCurrent: new FormControl(this.storedInputs.flawedCurrent, { nonNullable: true }),
    flawedTarget: new FormControl(this.storedInputs.flawedTarget, { nonNullable: true }),
  });

  private readonly formValue = toSignal(
    this.form.valueChanges.pipe(startWith(this.form.getRawValue())),
    { initialValue: this.form.getRawValue() },
  );

  protected readonly remainingByType = computed(() => {
    const value = this.formValue();
    const remaining: Record<SignalDataType, number> = {
      fragmented: this.calculateRemaining(value.fragmentedCurrent, value.fragmentedTarget),
      incomplete: this.calculateRemaining(value.incompleteCurrent, value.incompleteTarget),
      flawed: this.calculateRemaining(value.flawedCurrent, value.flawedTarget),
    };
    return remaining;
  });
  protected readonly farmingPlan = computed(() => {
    const remaining = this.remainingByType();
    return this.signalDataFarming.getRemainingDays(
      remaining.fragmented,
      remaining.incomplete,
      remaining.flawed,
    );
  });
  protected readonly hasRemainingSignalData = computed(() => {
    const remaining = this.remainingByType();
    return remaining.fragmented > 0 || remaining.incomplete > 0 || remaining.flawed > 0;
  });

  public constructor() {
    effect(() => {
      const value = this.formValue();
      this.writeStorage(
        SIGNAL_DATA_FARMING_STORAGE_KEY,
        JSON.stringify({
          fragmentedCurrent: String(value.fragmentedCurrent ?? '0'),
          fragmentedTarget: String(value.fragmentedTarget ?? '0'),
          incompleteCurrent: String(value.incompleteCurrent ?? '0'),
          incompleteTarget: String(value.incompleteTarget ?? '0'),
          flawedCurrent: String(value.flawedCurrent ?? '0'),
          flawedTarget: String(value.flawedTarget ?? '0'),
        } satisfies SignalDataFarmStorage),
      );
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

    const storedValue = this.readStorage(SIGNAL_DATA_FARMING_STORAGE_KEY);
    if (storedValue === null) {
      return defaultValues;
    }

    try {
      const parsed = JSON.parse(storedValue) as Partial<SignalDataFarmStorage>;
      return {
        fragmentedCurrent: this.readStringValue(parsed.fragmentedCurrent, defaultValues.fragmentedCurrent),
        fragmentedTarget: this.readStringValue(parsed.fragmentedTarget, defaultValues.fragmentedTarget),
        incompleteCurrent: this.readStringValue(parsed.incompleteCurrent, defaultValues.incompleteCurrent),
        incompleteTarget: this.readStringValue(parsed.incompleteTarget, defaultValues.incompleteTarget),
        flawedCurrent: this.readStringValue(parsed.flawedCurrent, defaultValues.flawedCurrent),
        flawedTarget: this.readStringValue(parsed.flawedTarget, defaultValues.flawedTarget),
      };
    } catch {
      return defaultValues;
    }
  }

  private readStringValue(value: unknown, fallback: string): string {
    return typeof value === 'string' ? value : fallback;
  }

  private readStorage(key: string): string | null {
    if (typeof localStorage === 'undefined') {
      return null;
    }

    try {
      return localStorage.getItem(key);
    } catch {
      return null;
    }
  }

  private writeStorage(key: string, value: string): void {
    if (typeof localStorage === 'undefined') {
      return;
    }

    try {
      localStorage.setItem(key, value);
    } catch {
      // Ignore storage failures.
    }
  }
}
