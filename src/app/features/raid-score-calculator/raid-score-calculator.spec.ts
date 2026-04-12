import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { RaidScoreCalculator } from './raid-score-calculator';

describe('RaidScoreCalculator', () => {
  const storageKey = 'raid.score-calculator.inputs';
  let storage: Storage;

  beforeEach(async () => {
    const data = new Map<string, string>();
    storage = {
      get length(): number {
        return data.size;
      },
      clear(): void {
        data.clear();
      },
      getItem(key: string): string | null {
        return data.has(key) ? data.get(key)! : null;
      },
      key(index: number): string | null {
        return Array.from(data.keys())[index] ?? null;
      },
      removeItem(key: string): void {
        data.delete(key);
      },
      setItem(key: string, value: string): void {
        data.set(key, value);
      },
    };

    Object.defineProperty(globalThis, 'localStorage', {
      configurable: true,
      value: storage,
    });

    await TestBed.configureTestingModule({
      imports: [RaidScoreCalculator],
      providers: [provideRouter([])],
    }).compileComponents();
  });

  it('should create', () => {
    const fixture = TestBed.createComponent(RaidScoreCalculator);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should initialize all tier fields to 0', () => {
    const fixture = TestBed.createComponent(RaidScoreCalculator);
    fixture.detectChanges();

    const tierInputIds = [
      '#tier0Teams',
      '#tier1Teams',
      '#tier2Teams',
      '#tier3Teams',
      '#tier4Teams',
      '#tier5Teams',
      '#tier6Teams',
      '#tier7Teams',
    ];
    for (const tierInputId of tierInputIds) {
      const input = fixture.nativeElement.querySelector(tierInputId) as HTMLInputElement;
      expect(input.value).toBe('0');
      expect(input.min).toBe('0');
      expect(input.max).toBe('5');
    }
  });

  it('should suggest a team mix from target score', async () => {
    const fixture = TestBed.createComponent(RaidScoreCalculator);
    fixture.detectChanges();

    const targetScoreInput = fixture.nativeElement.querySelector('#targetScore') as HTMLInputElement;
    targetScoreInput.value = '1350000';
    targetScoreInput.dispatchEvent(new Event('input'));

    fixture.detectChanges();
    await fixture.whenStable();

    const resultText = fixture.nativeElement.querySelector('.target-result')?.textContent ?? '';
    expect(resultText).toContain('T0: 3');
    expect(resultText).toContain('T1: 1');
    expect(resultText).toContain('Achieved Score: 1,350,000');
  });

  it('should apply suggested teams to tier fields', async () => {
    const fixture = TestBed.createComponent(RaidScoreCalculator);
    fixture.detectChanges();

    const targetScoreInput = fixture.nativeElement.querySelector('#targetScore') as HTMLInputElement;
    targetScoreInput.value = '1350000';
    targetScoreInput.dispatchEvent(new Event('input'));

    fixture.detectChanges();
    await fixture.whenStable();

    const applyButton = fixture.nativeElement.querySelector('.target-result button') as HTMLButtonElement;
    applyButton.click();

    fixture.detectChanges();
    await fixture.whenStable();

    const tier0Input = fixture.nativeElement.querySelector('#tier0Teams') as HTMLInputElement;
    const tier1Input = fixture.nativeElement.querySelector('#tier1Teams') as HTMLInputElement;
    const scoreText = fixture.nativeElement.querySelector('.score')?.textContent ?? '';
    expect(tier0Input.value).toBe('3');
    expect(tier1Input.value).toBe('1');
    expect(scoreText).toContain('1,350,000');
  });

  it('should calculate relic materials for the current team selection', async () => {
    const fixture = TestBed.createComponent(RaidScoreCalculator);
    fixture.detectChanges();

    const tier3Input = fixture.nativeElement.querySelector('#tier3Teams') as HTMLInputElement;
    tier3Input.value = '1';
    tier3Input.dispatchEvent(new Event('input'));

    fixture.detectChanges();
    await fixture.whenStable();

    const tableText = fixture.nativeElement.querySelector('.materials-section table')?.textContent ?? '';
    expect(tableText).toContain('Fragmented Signal Data');
    expect(tableText).toContain('175');
    expect(tableText).toContain('Incomplete Signal Data');
    expect(tableText).toContain('75');
    expect(tableText).toContain('Carbonite Circuit Board');
    expect(tableText).toContain('500');
    expect(tableText).toContain('Bronzium Wiring');
    expect(tableText).toContain('400');
    expect(tableText).toContain('Chromium Transistor');
    expect(tableText).toContain('100');
  });

  it('should persist entered values to local storage', async () => {
    const fixture = TestBed.createComponent(RaidScoreCalculator);
    fixture.detectChanges();

    const tier0Input = fixture.nativeElement.querySelector('#tier0Teams') as HTMLInputElement;
    const tier1Input = fixture.nativeElement.querySelector('#tier1Teams') as HTMLInputElement;
    const targetScoreInput = fixture.nativeElement.querySelector('#targetScore') as HTMLInputElement;

    tier0Input.value = '2';
    tier0Input.dispatchEvent(new Event('input'));
    tier1Input.value = '1';
    tier1Input.dispatchEvent(new Event('input'));
    targetScoreInput.value = '1000000';
    targetScoreInput.dispatchEvent(new Event('input'));

    fixture.detectChanges();
    await fixture.whenStable();

    const stored = storage.getItem(storageKey);
    expect(stored).not.toBeNull();
    expect(stored).toContain('"tier0Teams":"2"');
    expect(stored).toContain('"tier1Teams":"1"');
    expect(stored).toContain('"targetScore":"1000000"');
  });

  it('should load saved values from local storage on init', () => {
    storage.setItem(
      storageKey,
      JSON.stringify({
        tier0Teams: '3',
        tier1Teams: '1',
        tier2Teams: '0',
        tier3Teams: '0',
        tier4Teams: '0',
        tier5Teams: '0',
        tier6Teams: '0',
        tier7Teams: '0',
        targetScore: '1350000',
      }),
    );

    const fixture = TestBed.createComponent(RaidScoreCalculator);
    fixture.detectChanges();

    const tier0Input = fixture.nativeElement.querySelector('#tier0Teams') as HTMLInputElement;
    const tier1Input = fixture.nativeElement.querySelector('#tier1Teams') as HTMLInputElement;
    const targetScoreInput = fixture.nativeElement.querySelector('#targetScore') as HTMLInputElement;
    const scoreText = fixture.nativeElement.querySelector('.score')?.textContent ?? '';

    expect(tier0Input.value).toBe('3');
    expect(tier1Input.value).toBe('1');
    expect(targetScoreInput.value).toBe('1350000');
    expect(scoreText).toContain('1,350,000');
  });

  it('should calculate score in real time as values are entered', async () => {
    const fixture = TestBed.createComponent(RaidScoreCalculator);
    fixture.detectChanges();

    const tier0Input = fixture.nativeElement.querySelector('#tier0Teams') as HTMLInputElement;
    const tier1Input = fixture.nativeElement.querySelector('#tier1Teams') as HTMLInputElement;

    tier0Input.value = '3';
    tier0Input.dispatchEvent(new Event('input'));
    tier1Input.value = '1';
    tier1Input.dispatchEvent(new Event('input'));

    fixture.detectChanges();
    await fixture.whenStable();

    const scoreText = fixture.nativeElement.querySelector('.score')?.textContent ?? '';
    const errorText = fixture.nativeElement.querySelector('#score-error')?.textContent ?? '';

    expect(scoreText).toContain('1,350,000');
    expect(errorText.trim()).toBe('');
  });

  it('should allow zero for a tier when total teams remain valid', async () => {
    const fixture = TestBed.createComponent(RaidScoreCalculator);
    fixture.detectChanges();

    const tier0Input = fixture.nativeElement.querySelector('#tier0Teams') as HTMLInputElement;
    const tier1Input = fixture.nativeElement.querySelector('#tier1Teams') as HTMLInputElement;
    tier0Input.value = '1';
    tier0Input.dispatchEvent(new Event('input'));
    tier1Input.value = '0';
    tier1Input.dispatchEvent(new Event('input'));

    fixture.detectChanges();
    await fixture.whenStable();

    const errorText = fixture.nativeElement.querySelector('#score-error')?.textContent ?? '';
    const scoreText = fixture.nativeElement.querySelector('.score')?.textContent ?? '';
    expect(errorText.trim()).toBe('');
    expect(scoreText).toContain('300,000');
  });

  it('should show an error when a negative number is entered', async () => {
    const fixture = TestBed.createComponent(RaidScoreCalculator);
    fixture.detectChanges();

    const tier0Input = fixture.nativeElement.querySelector('#tier0Teams') as HTMLInputElement;
    tier0Input.value = '-1';
    tier0Input.dispatchEvent(new Event('input'));

    fixture.detectChanges();
    await fixture.whenStable();

    const errorText = fixture.nativeElement.querySelector('#score-error')?.textContent ?? '';
    expect(errorText).toContain('Each tier must be a whole number greater than or equal to 0.');
  });

  it('should show an error when total teams exceed five', async () => {
    const fixture = TestBed.createComponent(RaidScoreCalculator);
    fixture.detectChanges();

    const tier0Input = fixture.nativeElement.querySelector('#tier0Teams') as HTMLInputElement;
    const tier1Input = fixture.nativeElement.querySelector('#tier1Teams') as HTMLInputElement;
    tier0Input.value = '3';
    tier0Input.dispatchEvent(new Event('input'));
    tier1Input.value = '3';
    tier1Input.dispatchEvent(new Event('input'));

    fixture.detectChanges();
    await fixture.whenStable();

    const errorText = fixture.nativeElement.querySelector('#score-error')?.textContent ?? '';
    expect(errorText).toContain('Total teams must be between 1 and 5.');
  });
});
