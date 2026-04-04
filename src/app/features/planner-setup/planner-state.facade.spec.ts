import { TestBed } from '@angular/core/testing';
import { vi } from 'vitest';
import { League } from '../../gac/league.enum';
import { PlannerStateFacade } from './planner-state.facade';
import {
  DEFENSE_PLAN_STORAGE_KEY,
  DEFENSE_PLAN_UPDATED_AT_STORAGE_KEY,
  LEAGUE_STORAGE_KEY,
} from './planner-storage.constants';

describe('PlannerStateFacade', () => {
  let storage: Storage;

  beforeEach(() => {
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
        matches: false,
        media: '(prefers-color-scheme: dark)',
        onchange: null,
        addEventListener: vi.fn(),
        removeEventListener: vi.fn(),
        addListener: vi.fn(),
        removeListener: vi.fn(),
        dispatchEvent: vi.fn().mockReturnValue(true),
      })),
    });

    TestBed.configureTestingModule({});
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  it('should keep dependent zones locked until source zone teams are all defeated', () => {
    const facade = TestBed.inject(PlannerStateFacade);
    TestBed.flushEffects();

    expect(facade.isZoneUnlocked('south-back')).toBe(false);

    const southFrontSlots =
      facade.plannerRules().zones.find((zone) => zone.zoneId === 'south-front')?.slots ?? 0;
    for (let index = 0; index < southFrontSlots; index += 1) {
      facade.setDefenseDefeated('south-front', index, true);
    }
    TestBed.flushEffects();

    expect(facade.isZoneUnlocked('south-back')).toBe(true);
  });

  it('should clear expired defense plan on initialization', () => {
    const now = Date.now();
    vi.spyOn(Date, 'now').mockReturnValue(now);

    storage.setItem(
      DEFENSE_PLAN_STORAGE_KEY,
      JSON.stringify({
        north: {
          teams: ['Old Team'],
          defeated: [false],
          attackOptions: [['Old Counter']],
          attackOptionDrafts: ['Old Draft'],
        },
      }),
    );
    storage.setItem(
      DEFENSE_PLAN_UPDATED_AT_STORAGE_KEY,
      String(now - (24 * 60 * 60 * 1000 + 1)),
    );

    const facade = TestBed.inject(PlannerStateFacade);
    TestBed.flushEffects();
    const northSlot = facade.zoneCards().find((zone) => zone.zoneId === 'north')?.teamSlots[0];

    expect(northSlot?.teamName).toBe('');
    expect(northSlot?.attackOptions).toEqual([]);
    expect(northSlot?.attackOptionDraft).toBe('');
  });

  it('should persist selected league changes to storage', () => {
    const facade = TestBed.inject(PlannerStateFacade);
    TestBed.flushEffects();

    facade.setLeague(League.Kyber);
    TestBed.flushEffects();

    expect(storage.getItem(LEAGUE_STORAGE_KEY)).toBe(League.Kyber);
  });
});
