import { ChangeDetectionStrategy, Component, computed, effect, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { startWith } from 'rxjs';
import { toSignal } from '@angular/core/rxjs-interop';
import { RaidScores, RaidTeamPlan } from '../../raid/raid-scores';
import { RelicLevel, RelicRequirements } from '../../raid/relic-requirements';

type TierControlName =
  | 'tier0Teams'
  | 'tier1Teams'
  | 'tier2Teams'
  | 'tier3Teams'
  | 'tier4Teams'
  | 'tier5Teams'
  | 'tier6Teams'
  | 'tier7Teams';

type RaidScoreForm = {
  [Key in TierControlName]: FormControl<string>;
};

interface RaidScoreCalculatorStorage {
  tier0Teams: string;
  tier1Teams: string;
  tier2Teams: string;
  tier3Teams: string;
  tier4Teams: string;
  tier5Teams: string;
  tier6Teams: string;
  tier7Teams: string;
  targetScore: string;
}

const RAID_SCORE_CALCULATOR_STORAGE_KEY = 'raid.score-calculator.inputs';
const CHARACTERS_PER_TEAM = 5;
const materialDefinitions = [
  { key: 'fragmentedSignalData', label: 'Fragmented Signal Data' },
  { key: 'incompleteSignalData', label: 'Incomplete Signal Data' },
  { key: 'flawedSignalData', label: 'Flawed Signal Data' },
  { key: 'corruptedSignalData', label: 'Corrupted Signal Data' },
  { key: 'carboniteCircuitBoard', label: 'Carbonite Circuit Board' },
  { key: 'bronziumWiring', label: 'Bronzium Wiring' },
  { key: 'chromiumTransistor', label: 'Chromium Transistor' },
  { key: 'aurodiumHeatsink', label: 'Aurodium Heatsink' },
  { key: 'electriumConductor', label: 'Electrium Conductor' },
  { key: 'zinbiddleCard', label: 'Zinbiddle Card' },
  { key: 'impulseDetector', label: 'Impulse Detector' },
  { key: 'aeromagnifier', label: 'Aeromagnifier' },
  { key: 'gyrdaKeypad', label: 'Gyrda Keypad' },
  { key: 'droidBrain', label: 'Droid Brain' },
  { key: 'coaxialServomotors', label: 'Coaxial Servomotors' },
] as const satisfies ReadonlyArray<{ key: keyof Omit<RelicLevel, 'level'>; label: string }>;

@Component({
  selector: 'app-raid-score-calculator',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './raid-score-calculator.html',
  styleUrl: './raid-score-calculator.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RaidScoreCalculator {
  private readonly raidScores = inject(RaidScores);
  private readonly relicRequirements = inject(RelicRequirements);
  private readonly storedInputs = this.readStoredInputs();

  protected readonly tierFields: ReadonlyArray<{
    readonly label: string;
    readonly controlName: TierControlName;
    readonly required: string;
  }> = [
    { label: 'Tier 0 Teams', controlName: 'tier0Teams', required: 'Gear XI' },
    { label: 'Tier 1 Teams', controlName: 'tier1Teams', required: 'Gear XII' },
    { label: 'Tier 2 Teams', controlName: 'tier2Teams', required: 'Relic 1' },
    { label: 'Tier 3 Teams', controlName: 'tier3Teams', required: 'Relic 3' },
    { label: 'Tier 4 Teams', controlName: 'tier4Teams', required: 'Relic 5' },
    { label: 'Tier 5 Teams', controlName: 'tier5Teams', required: 'Relic 7' },
    { label: 'Tier 6 Teams', controlName: 'tier6Teams', required: 'Relic 8' },
    { label: 'Tier 7 Teams', controlName: 'tier7Teams', required: 'Relic 9' },
  ];
  private readonly tierRelicLevels = this.raidScores.getRaidTiers().map((tier) => tier.relicLevel);

  protected readonly form = new FormGroup<RaidScoreForm>({
    tier0Teams: new FormControl(this.storedInputs.tier0Teams, { nonNullable: true }),
    tier1Teams: new FormControl(this.storedInputs.tier1Teams, { nonNullable: true }),
    tier2Teams: new FormControl(this.storedInputs.tier2Teams, { nonNullable: true }),
    tier3Teams: new FormControl(this.storedInputs.tier3Teams, { nonNullable: true }),
    tier4Teams: new FormControl(this.storedInputs.tier4Teams, { nonNullable: true }),
    tier5Teams: new FormControl(this.storedInputs.tier5Teams, { nonNullable: true }),
    tier6Teams: new FormControl(this.storedInputs.tier6Teams, { nonNullable: true }),
    tier7Teams: new FormControl(this.storedInputs.tier7Teams, { nonNullable: true }),
  });
  protected readonly targetScoreControl = new FormControl(this.storedInputs.targetScore, { nonNullable: true });

  private readonly formValue = toSignal(
    this.form.valueChanges.pipe(startWith(this.form.getRawValue())),
    { initialValue: this.form.getRawValue() },
  );
  private readonly targetScoreValue = toSignal(
    this.targetScoreControl.valueChanges.pipe(startWith(this.targetScoreControl.getRawValue())),
    { initialValue: this.targetScoreControl.getRawValue() },
  );

  public constructor() {
    effect(() => {
      const teamValues = this.formValue();
      const targetScore = this.targetScoreValue();
      this.writeStorage(
        RAID_SCORE_CALCULATOR_STORAGE_KEY,
        JSON.stringify({
          tier0Teams: String(teamValues.tier0Teams ?? '0'),
          tier1Teams: String(teamValues.tier1Teams ?? '0'),
          tier2Teams: String(teamValues.tier2Teams ?? '0'),
          tier3Teams: String(teamValues.tier3Teams ?? '0'),
          tier4Teams: String(teamValues.tier4Teams ?? '0'),
          tier5Teams: String(teamValues.tier5Teams ?? '0'),
          tier6Teams: String(teamValues.tier6Teams ?? '0'),
          tier7Teams: String(teamValues.tier7Teams ?? '0'),
          targetScore: String(targetScore ?? ''),
        } satisfies RaidScoreCalculatorStorage),
      );
    });
  }

  private readonly parsedCounts = computed(() => {
    const value = this.formValue();

    return this.tierFields.map((tierField) =>
      this.parseTeamCount(value[tierField.controlName]),
    );
  });

  protected readonly validationError = computed(() => {
    const parsedCounts = this.parsedCounts();
    const invalidTier = parsedCounts.find((item) => item.error !== null);

    if (invalidTier?.error) {
      return invalidTier.error;
    }

    const totalTeams = parsedCounts.reduce((total, item) => total + (item.value ?? 0), 0);
    if (totalTeams < 1 || totalTeams > 5) {
      return 'Total teams must be between 1 and 5.';
    }

    return null;
  });

  protected readonly totalTeams = computed(() =>
    this.parsedCounts().reduce((total, item) => total + (item.value ?? 0), 0),
  );

  protected readonly score = computed(() => {
    if (this.validationError() !== null) {
      return 0;
    }

    const values = this.parsedCounts().map((item) =>
      item.value === 0 ? undefined : item.value,
    );
    return this.raidScores.calculateScore(
      values[0],
      values[1],
      values[2],
      values[3],
      values[4],
      values[5],
      values[6],
      values[7],
    );
  });

  protected readonly formattedScore = computed(() => this.score().toLocaleString('en-US'));
  protected readonly relicMaterialTotals = computed(() => {
    const totals: Omit<RelicLevel, 'level'> = {
      fragmentedSignalData: 0,
      incompleteSignalData: 0,
      flawedSignalData: 0,
      corruptedSignalData: 0,
      carboniteCircuitBoard: 0,
      bronziumWiring: 0,
      chromiumTransistor: 0,
      aurodiumHeatsink: 0,
      electriumConductor: 0,
      zinbiddleCard: 0,
      impulseDetector: 0,
      aeromagnifier: 0,
      gyrdaKeypad: 0,
      droidBrain: 0,
      coaxialServomotors: 0,
    };

    if (this.validationError() !== null) {
      return totals;
    }

    const parsedCounts = this.parsedCounts();
    for (let tierIndex = 0; tierIndex < parsedCounts.length; tierIndex += 1) {
      const teamCount = parsedCounts[tierIndex]?.value ?? 0;
      if (teamCount <= 0) {
        continue;
      }

      const relicLevel = this.tierRelicLevels[tierIndex] ?? 0;
      const requirementsForOneTeam = this.relicRequirements.getRequirements(0, relicLevel);
      for (const materialDefinition of materialDefinitions) {
        totals[materialDefinition.key] +=
          teamCount * CHARACTERS_PER_TEAM * requirementsForOneTeam[materialDefinition.key];
      }
    }

    return totals;
  });
  protected readonly relicMaterialRows = computed(() =>
    materialDefinitions.map((materialDefinition) => ({
      label: materialDefinition.label,
      value: this.relicMaterialTotals()[materialDefinition.key],
    })),
  );
  protected readonly targetPlanResult = computed(() => {
    const parsedTarget = this.parseTargetScore(this.targetScoreValue());
    if (parsedTarget.error !== null) {
      return { error: parsedTarget.error, plan: null as RaidTeamPlan | null };
    }

    if (parsedTarget.value === undefined) {
      return { error: null, plan: null as RaidTeamPlan | null };
    }

    try {
      return { error: null, plan: this.raidScores.calculateTeamsForTarget(parsedTarget.value) };
    } catch (error) {
      const message = error instanceof Error ? error.message : 'Invalid target score.';
      return { error: message, plan: null as RaidTeamPlan | null };
    }
  });

  protected readonly targetPlanError = computed(() => this.targetPlanResult().error);
  protected readonly targetPlan = computed(() => this.targetPlanResult().plan);

  protected applyTargetPlan(): void {
    const plan = this.targetPlan();
    if (plan === null) {
      return;
    }

    this.form.setValue({
      tier0Teams: String(plan.tier0Teams),
      tier1Teams: String(plan.tier1Teams),
      tier2Teams: String(plan.tier2Teams),
      tier3Teams: String(plan.tier3Teams),
      tier4Teams: String(plan.tier4Teams),
      tier5Teams: String(plan.tier5Teams),
      tier6Teams: String(plan.tier6Teams),
      tier7Teams: String(plan.tier7Teams),
    });
  }

  private parseTeamCount(rawValue: unknown): { value: number | undefined; error: string | null } {
    if (rawValue === undefined || rawValue === null) {
      return { value: undefined, error: null };
    }

    if (typeof rawValue === 'string' && rawValue.trim() === '') {
      return { value: undefined, error: null };
    }

    const parsedValue = Number(rawValue);
    if (!Number.isInteger(parsedValue) || parsedValue < 0) {
      return { value: undefined, error: 'Each tier must be a whole number greater than or equal to 0.' };
    }

    return { value: parsedValue, error: null };
  }

  private parseTargetScore(rawValue: unknown): { value: number | undefined; error: string | null } {
    if (rawValue === undefined || rawValue === null) {
      return { value: undefined, error: null };
    }

    if (typeof rawValue === 'string' && rawValue.trim() === '') {
      return { value: undefined, error: null };
    }

    const parsedValue = Number(rawValue);
    if (!Number.isFinite(parsedValue)) {
      return { value: undefined, error: 'Target score must be a finite number.' };
    }

    if (parsedValue <= 0) {
      return { value: undefined, error: 'Target score must be greater than 0.' };
    }

    return { value: parsedValue, error: null };
  }

  private readStoredInputs(): RaidScoreCalculatorStorage {
    const defaults: RaidScoreCalculatorStorage = {
      tier0Teams: '0',
      tier1Teams: '0',
      tier2Teams: '0',
      tier3Teams: '0',
      tier4Teams: '0',
      tier5Teams: '0',
      tier6Teams: '0',
      tier7Teams: '0',
      targetScore: '',
    };

    const stored = this.readStorage(RAID_SCORE_CALCULATOR_STORAGE_KEY);
    if (!stored) {
      return defaults;
    }

    try {
      const parsed = JSON.parse(stored) as Partial<RaidScoreCalculatorStorage> | null;
      if (!parsed || typeof parsed !== 'object') {
        return defaults;
      }

      return {
        tier0Teams: this.parseStoredInputValue(parsed.tier0Teams, defaults.tier0Teams),
        tier1Teams: this.parseStoredInputValue(parsed.tier1Teams, defaults.tier1Teams),
        tier2Teams: this.parseStoredInputValue(parsed.tier2Teams, defaults.tier2Teams),
        tier3Teams: this.parseStoredInputValue(parsed.tier3Teams, defaults.tier3Teams),
        tier4Teams: this.parseStoredInputValue(parsed.tier4Teams, defaults.tier4Teams),
        tier5Teams: this.parseStoredInputValue(parsed.tier5Teams, defaults.tier5Teams),
        tier6Teams: this.parseStoredInputValue(parsed.tier6Teams, defaults.tier6Teams),
        tier7Teams: this.parseStoredInputValue(parsed.tier7Teams, defaults.tier7Teams),
        targetScore: this.parseStoredInputValue(parsed.targetScore, defaults.targetScore),
      };
    } catch {
      return defaults;
    }
  }

  private parseStoredInputValue(value: unknown, fallback: string): string {
    if (typeof value === 'string') {
      return value;
    }

    if (typeof value === 'number' && Number.isFinite(value)) {
      return String(value);
    }

    return fallback;
  }

  private readStorage(key: string): string | null {
    try {
      const storage = globalThis.localStorage as Partial<Storage> | undefined;
      return typeof storage?.getItem === 'function' ? storage.getItem(key) : null;
    } catch {
      return null;
    }
  }

  private writeStorage(key: string, value: string): void {
    try {
      const storage = globalThis.localStorage as Partial<Storage> | undefined;
      if (typeof storage?.setItem === 'function') {
        storage.setItem(key, value);
      }
    } catch {
      // Ignore storage failures to keep the calculator usable.
    }
  }
}
