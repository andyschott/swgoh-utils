import { TestBed } from '@angular/core/testing';
import { vi } from 'vitest';
import { GacMode } from '../../gac/gac-mode.enum';
import { League } from '../../gac/league.enum';
import {
  DEFENSE_PLAN_STORAGE_KEY,
  GAC_MODE_STORAGE_KEY,
  LEAGUE_STORAGE_KEY,
  PlannerSetup,
} from './planner-setup';

describe('PlannerSetup', () => {
  let storage: Storage;
  let mediaQueryMatchesDark = false;
  let mediaQueryChangeListeners: Array<(event: MediaQueryListEvent) => void> = [];

  beforeEach(async () => {
    mediaQueryMatchesDark = false;
    mediaQueryChangeListeners = [];

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

    Object.defineProperty(globalThis, 'matchMedia', {
      configurable: true,
      value: vi.fn().mockImplementation(() => ({
        get matches() {
          return mediaQueryMatchesDark;
        },
        media: '(prefers-color-scheme: dark)',
        onchange: null,
        addEventListener: (_: 'change', handler: (event: MediaQueryListEvent) => void) => {
          mediaQueryChangeListeners.push(handler);
        },
        removeEventListener: (_: 'change', handler: (event: MediaQueryListEvent) => void) => {
          mediaQueryChangeListeners = mediaQueryChangeListeners.filter(
            (listener) => listener !== handler,
          );
        },
        addListener: (handler: (event: MediaQueryListEvent) => void) => {
          mediaQueryChangeListeners.push(handler);
        },
        removeListener: (handler: (event: MediaQueryListEvent) => void) => {
          mediaQueryChangeListeners = mediaQueryChangeListeners.filter(
            (listener) => listener !== handler,
          );
        },
        dispatchEvent: () => true,
      })),
    });

    await TestBed.configureTestingModule({
      imports: [PlannerSetup],
    }).compileComponents();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  it('should render planner setup heading and default selections', async () => {
    const fixture = TestBed.createComponent(PlannerSetup);
    await fixture.whenStable();
    fixture.detectChanges();

    const host = fixture.nativeElement as HTMLElement;
    expect(host.querySelector('h1')?.textContent).toContain('GAC Attack Planner Setup');

    const modeSelect = host.querySelector('#gac-mode') as HTMLSelectElement | null;
    const leagueSelect = host.querySelector('#league') as HTMLSelectElement | null;
    expect(modeSelect?.value).toBe(GacMode.FiveVFive);
    expect(leagueSelect?.value).toBe(League.Carbonite);
  });

  it('should restore mode and league from local storage', () => {
    storage.setItem(GAC_MODE_STORAGE_KEY, GacMode.ThreeVThree);
    storage.setItem(LEAGUE_STORAGE_KEY, League.Kyber);

    const fixture = TestBed.createComponent(PlannerSetup);
    fixture.detectChanges();

    const host = fixture.nativeElement as HTMLElement;
    const modeSelect = host.querySelector('#gac-mode') as HTMLSelectElement | null;
    const leagueSelect = host.querySelector('#league') as HTMLSelectElement | null;

    expect(modeSelect?.value).toBe(GacMode.ThreeVThree);
    expect(leagueSelect?.value).toBe(League.Kyber);
  });

  it('should persist selected mode and league from config panel changes', () => {
    const fixture = TestBed.createComponent(PlannerSetup);
    fixture.detectChanges();

    const host = fixture.nativeElement as HTMLElement;
    const modeSelect = host.querySelector('#gac-mode') as HTMLSelectElement;
    const leagueSelect = host.querySelector('#league') as HTMLSelectElement;

    modeSelect.value = GacMode.ThreeVThree;
    modeSelect.dispatchEvent(new Event('change'));

    leagueSelect.value = League.Kyber;
    leagueSelect.dispatchEvent(new Event('change'));

    fixture.detectChanges();

    expect(storage.getItem(GAC_MODE_STORAGE_KEY)).toBe(GacMode.ThreeVThree);
    expect(storage.getItem(LEAGUE_STORAGE_KEY)).toBe(League.Kyber);
  });

  it('should collapse and expand zones via the zone card toggle', () => {
    const fixture = TestBed.createComponent(PlannerSetup);
    fixture.detectChanges();

    const host = fixture.nativeElement as HTMLElement;
    const toggle = host.querySelector('#zone-card-north .zone-toggle') as HTMLButtonElement;

    expect(host.querySelector('#zone-body-north')).not.toBeNull();

    toggle.click();
    fixture.detectChanges();
    expect(host.querySelector('#zone-body-north')).toBeNull();

    toggle.click();
    fixture.detectChanges();
    expect(host.querySelector('#zone-body-north')).not.toBeNull();
  });

  it('should clear defense entries while keeping mode and league selections', () => {
    const fixture = TestBed.createComponent(PlannerSetup);
    fixture.detectChanges();

    const host = fixture.nativeElement as HTMLElement;
    const modeSelect = host.querySelector('#gac-mode') as HTMLSelectElement;
    const leagueSelect = host.querySelector('#league') as HTMLSelectElement;

    modeSelect.value = GacMode.ThreeVThree;
    modeSelect.dispatchEvent(new Event('change'));
    leagueSelect.value = League.Kyber;
    leagueSelect.dispatchEvent(new Event('change'));

    const teamInput = host.querySelector('#north-team-0') as HTMLInputElement;
    teamInput.value = 'Reva';
    teamInput.dispatchEvent(new Event('input'));

    const draftInput = host.querySelector('#north-attack-option-0') as HTMLInputElement;
    draftInput.value = 'Leia';
    draftInput.dispatchEvent(new Event('input'));

    const addButton = host.querySelector('#north-attack-option-0 + button') as HTMLButtonElement;
    addButton.click();
    fixture.detectChanges();

    const clearButton = host.querySelector('.clear-button') as HTMLButtonElement;
    clearButton.click();
    fixture.detectChanges();

    const refreshedTeamInput = host.querySelector('#north-team-0') as HTMLInputElement;
    expect(refreshedTeamInput.value).toBe('');
    expect(storage.getItem(GAC_MODE_STORAGE_KEY)).toBe(GacMode.ThreeVThree);
    expect(storage.getItem(LEAGUE_STORAGE_KEY)).toBe(League.Kyber);
    expect(storage.getItem(DEFENSE_PLAN_STORAGE_KEY)).not.toContain('Reva');
    expect(storage.getItem(DEFENSE_PLAN_STORAGE_KEY)).not.toContain('Leia');
  });

  it('should reflect device theme and update when preference changes', () => {
    mediaQueryMatchesDark = false;

    const fixture = TestBed.createComponent(PlannerSetup);
    fixture.detectChanges();

    expect((fixture.nativeElement as HTMLElement).getAttribute('data-theme')).toBe('light');

    mediaQueryMatchesDark = true;
    for (const listener of mediaQueryChangeListeners) {
      listener({ matches: true } as MediaQueryListEvent);
    }
    fixture.detectChanges();

    expect((fixture.nativeElement as HTMLElement).getAttribute('data-theme')).toBe('dark');
  });
});
