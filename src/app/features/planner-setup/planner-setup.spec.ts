import { TestBed } from '@angular/core/testing';
import { GacMode } from '../../gac/gac-mode.enum';
import { League } from '../../gac/league.enum';
import { GAC_MODE_STORAGE_KEY, LEAGUE_STORAGE_KEY, PlannerSetup } from './planner-setup';

describe('PlannerSetup', () => {
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
      imports: [PlannerSetup],
    }).compileComponents();
  });

  it('should render planner setup heading', async () => {
    const fixture = TestBed.createComponent(PlannerSetup);
    await fixture.whenStable();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('GAC Attack Planner Setup');
  });

  it('should initialize enum defaults for mode and league', () => {
    const fixture = TestBed.createComponent(PlannerSetup);
    const component = fixture.componentInstance;
    expect(component['selectedMode']()).toBe(GacMode.FiveVFive);
    expect(component['selectedLeague']()).toBe(League.Carbonite);
  });

  it('should render 5v5 as the selected mode by default', async () => {
    const fixture = TestBed.createComponent(PlannerSetup);
    await fixture.whenStable();
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const modeSelect = compiled.querySelector('#gac-mode') as HTMLSelectElement | null;

    expect(modeSelect).not.toBeNull();
    expect(modeSelect?.value).toBe(GacMode.FiveVFive);
  });

  it('should restore mode and league from local storage', () => {
    storage.setItem(GAC_MODE_STORAGE_KEY, GacMode.ThreeVThree);
    storage.setItem(LEAGUE_STORAGE_KEY, League.Kyber);

    const fixture = TestBed.createComponent(PlannerSetup);
    const component = fixture.componentInstance;

    expect(component['selectedMode']()).toBe(GacMode.ThreeVThree);
    expect(component['selectedLeague']()).toBe(League.Kyber);
  });

  it('should persist selected mode and league to local storage', () => {
    const fixture = TestBed.createComponent(PlannerSetup);
    const component = fixture.componentInstance;
    fixture.detectChanges();

    component['selectedMode'].set(GacMode.ThreeVThree);
    component['selectedLeague'].set(League.Kyber);
    fixture.detectChanges();

    expect(storage.getItem(GAC_MODE_STORAGE_KEY)).toBe(GacMode.ThreeVThree);
    expect(storage.getItem(LEAGUE_STORAGE_KEY)).toBe(League.Kyber);
  });
});
