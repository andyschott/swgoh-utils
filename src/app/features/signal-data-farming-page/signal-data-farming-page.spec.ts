import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { SignalDataFarmingPage } from './signal-data-farming-page';

describe('SignalDataFarmingPage', () => {
  const storageKey = 'signal-data-farming.inputs';
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
      imports: [SignalDataFarmingPage],
      providers: [provideRouter([])],
    }).compileComponents();
  });

  it('should create', () => {
    const fixture = TestBed.createComponent(SignalDataFarmingPage);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should initialize all inputs to 0', () => {
    const fixture = TestBed.createComponent(SignalDataFarmingPage);
    fixture.detectChanges();

    const inputIds = [
      '#fragmentedCurrent',
      '#fragmentedTarget',
      '#incompleteCurrent',
      '#incompleteTarget',
      '#flawedCurrent',
      '#flawedTarget',
    ];

    for (const inputId of inputIds) {
      const input = fixture.nativeElement.querySelector(inputId) as HTMLInputElement;
      expect(input.value).toBe('0');
      expect(input.min).toBe('0');
    }
  });

  it('should calculate remaining signal data in real time', async () => {
    const fixture = TestBed.createComponent(SignalDataFarmingPage);
    fixture.detectChanges();

    const fragmentedCurrent = fixture.nativeElement.querySelector('#fragmentedCurrent') as HTMLInputElement;
    const fragmentedTarget = fixture.nativeElement.querySelector('#fragmentedTarget') as HTMLInputElement;
    const incompleteCurrent = fixture.nativeElement.querySelector('#incompleteCurrent') as HTMLInputElement;
    const incompleteTarget = fixture.nativeElement.querySelector('#incompleteTarget') as HTMLInputElement;
    const flawedCurrent = fixture.nativeElement.querySelector('#flawedCurrent') as HTMLInputElement;
    const flawedTarget = fixture.nativeElement.querySelector('#flawedTarget') as HTMLInputElement;

    fragmentedCurrent.value = '55';
    fragmentedCurrent.dispatchEvent(new Event('input'));
    fragmentedTarget.value = '100';
    fragmentedTarget.dispatchEvent(new Event('input'));

    incompleteCurrent.value = '12';
    incompleteCurrent.dispatchEvent(new Event('input'));
    incompleteTarget.value = '61';
    incompleteTarget.dispatchEvent(new Event('input'));

    flawedCurrent.value = '7';
    flawedCurrent.dispatchEvent(new Event('input'));
    flawedTarget.value = '40';
    flawedTarget.dispatchEvent(new Event('input'));

    fixture.detectChanges();
    await fixture.whenStable();

    const bodyText = fixture.nativeElement.querySelector('tbody')?.textContent ?? '';
    expect(bodyText).toContain('45');
    expect(bodyText).toContain('49');
    expect(bodyText).toContain('33');
  });

  it('should never show a negative remaining amount', async () => {
    const fixture = TestBed.createComponent(SignalDataFarmingPage);
    fixture.detectChanges();

    const fragmentedCurrent = fixture.nativeElement.querySelector('#fragmentedCurrent') as HTMLInputElement;
    const fragmentedTarget = fixture.nativeElement.querySelector('#fragmentedTarget') as HTMLInputElement;

    fragmentedCurrent.value = '200';
    fragmentedCurrent.dispatchEvent(new Event('input'));
    fragmentedTarget.value = '20';
    fragmentedTarget.dispatchEvent(new Event('input'));

    fixture.detectChanges();
    await fixture.whenStable();

    const firstRowCells = fixture.nativeElement.querySelectorAll('tbody tr')[0]?.querySelectorAll('td');
    const remainingText = firstRowCells?.[2]?.textContent?.trim() ?? '';
    expect(remainingText).toBe('0');
  });

  it('should calculate farming days and show nodes from the signal data farming service', async () => {
    const fixture = TestBed.createComponent(SignalDataFarmingPage);
    fixture.detectChanges();

    const fragmentedCurrent = fixture.nativeElement.querySelector('#fragmentedCurrent') as HTMLInputElement;
    const fragmentedTarget = fixture.nativeElement.querySelector('#fragmentedTarget') as HTMLInputElement;
    const incompleteCurrent = fixture.nativeElement.querySelector('#incompleteCurrent') as HTMLInputElement;
    const incompleteTarget = fixture.nativeElement.querySelector('#incompleteTarget') as HTMLInputElement;
    const flawedCurrent = fixture.nativeElement.querySelector('#flawedCurrent') as HTMLInputElement;
    const flawedTarget = fixture.nativeElement.querySelector('#flawedTarget') as HTMLInputElement;

    fragmentedCurrent.value = '55';
    fragmentedCurrent.dispatchEvent(new Event('input'));
    fragmentedTarget.value = '100';
    fragmentedTarget.dispatchEvent(new Event('input'));

    incompleteCurrent.value = '12';
    incompleteCurrent.dispatchEvent(new Event('input'));
    incompleteTarget.value = '61';
    incompleteTarget.dispatchEvent(new Event('input'));

    flawedCurrent.value = '7';
    flawedCurrent.dispatchEvent(new Event('input'));
    flawedTarget.value = '40';
    flawedTarget.dispatchEvent(new Event('input'));

    fixture.detectChanges();
    await fixture.whenStable();

    const farmingPlanText = fixture.nativeElement.querySelector('.farming-plan')?.textContent ?? '';
    expect(farmingPlanText).toContain('Estimated Days:');
    expect(farmingPlanText).toContain('4');
    expect(farmingPlanText).toContain('Nodes to Farm:');

    const nodeItems = Array.from(
      fixture.nativeElement.querySelectorAll('.farming-plan ol li'),
    ).map((item) => (item as HTMLLIElement).textContent?.trim());

    expect(nodeItems).toEqual(['9-F', '9-D', '8-C']);
  });

  it('should hide farming plan and show no-remaining message when all remaining amounts are zero', async () => {
    const fixture = TestBed.createComponent(SignalDataFarmingPage);
    fixture.detectChanges();

    const fragmentedCurrent = fixture.nativeElement.querySelector('#fragmentedCurrent') as HTMLInputElement;
    const fragmentedTarget = fixture.nativeElement.querySelector('#fragmentedTarget') as HTMLInputElement;
    const incompleteCurrent = fixture.nativeElement.querySelector('#incompleteCurrent') as HTMLInputElement;
    const incompleteTarget = fixture.nativeElement.querySelector('#incompleteTarget') as HTMLInputElement;
    const flawedCurrent = fixture.nativeElement.querySelector('#flawedCurrent') as HTMLInputElement;
    const flawedTarget = fixture.nativeElement.querySelector('#flawedTarget') as HTMLInputElement;

    fragmentedCurrent.value = '25';
    fragmentedCurrent.dispatchEvent(new Event('input'));
    fragmentedTarget.value = '25';
    fragmentedTarget.dispatchEvent(new Event('input'));

    incompleteCurrent.value = '12';
    incompleteCurrent.dispatchEvent(new Event('input'));
    incompleteTarget.value = '12';
    incompleteTarget.dispatchEvent(new Event('input'));

    flawedCurrent.value = '7';
    flawedCurrent.dispatchEvent(new Event('input'));
    flawedTarget.value = '7';
    flawedTarget.dispatchEvent(new Event('input'));

    fixture.detectChanges();
    await fixture.whenStable();

    const farmingPlan = fixture.nativeElement.querySelector('.farming-plan');
    const noRemainingMessage = fixture.nativeElement.querySelector('.no-remaining-signal-data');

    expect(farmingPlan).toBeNull();
    expect(noRemainingMessage?.textContent).toContain('You have no remaining signal data to farm.');
  });

  it('should persist entered values to local storage', async () => {
    const fixture = TestBed.createComponent(SignalDataFarmingPage);
    fixture.detectChanges();

    const fragmentedCurrent = fixture.nativeElement.querySelector('#fragmentedCurrent') as HTMLInputElement;
    const incompleteTarget = fixture.nativeElement.querySelector('#incompleteTarget') as HTMLInputElement;
    const flawedTarget = fixture.nativeElement.querySelector('#flawedTarget') as HTMLInputElement;

    fragmentedCurrent.value = '77';
    fragmentedCurrent.dispatchEvent(new Event('input'));
    incompleteTarget.value = '33';
    incompleteTarget.dispatchEvent(new Event('input'));
    flawedTarget.value = '111';
    flawedTarget.dispatchEvent(new Event('input'));

    fixture.detectChanges();
    await fixture.whenStable();

    const stored = storage.getItem(storageKey);
    expect(stored).not.toBeNull();
    expect(stored).toContain('"fragmentedCurrent":"77"');
    expect(stored).toContain('"incompleteTarget":"33"');
    expect(stored).toContain('"flawedTarget":"111"');
  });

  it('should load saved values from local storage on init', () => {
    storage.setItem(
      storageKey,
      JSON.stringify({
        fragmentedCurrent: '10',
        fragmentedTarget: '90',
        incompleteCurrent: '5',
        incompleteTarget: '40',
        flawedCurrent: '2',
        flawedTarget: '30',
      }),
    );

    const fixture = TestBed.createComponent(SignalDataFarmingPage);
    fixture.detectChanges();

    const fragmentedCurrent = fixture.nativeElement.querySelector('#fragmentedCurrent') as HTMLInputElement;
    const fragmentedTarget = fixture.nativeElement.querySelector('#fragmentedTarget') as HTMLInputElement;
    const incompleteCurrent = fixture.nativeElement.querySelector('#incompleteCurrent') as HTMLInputElement;
    const incompleteTarget = fixture.nativeElement.querySelector('#incompleteTarget') as HTMLInputElement;
    const flawedCurrent = fixture.nativeElement.querySelector('#flawedCurrent') as HTMLInputElement;
    const flawedTarget = fixture.nativeElement.querySelector('#flawedTarget') as HTMLInputElement;

    expect(fragmentedCurrent.value).toBe('10');
    expect(fragmentedTarget.value).toBe('90');
    expect(incompleteCurrent.value).toBe('5');
    expect(incompleteTarget.value).toBe('40');
    expect(flawedCurrent.value).toBe('2');
    expect(flawedTarget.value).toBe('30');
  });
});
