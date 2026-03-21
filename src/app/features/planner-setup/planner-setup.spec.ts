import { TestBed } from '@angular/core/testing';
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

  it('should keep South Back and Ships locked until dependency zones are fully defeated', () => {
    const fixture = TestBed.createComponent(PlannerSetup);
    const component = fixture.componentInstance;
    fixture.detectChanges();

    expect(component['isZoneUnlocked']('south-back')).toBe(false);
    expect(component['isZoneUnlocked']('ships')).toBe(false);

    const southFrontSlots =
      component['plannerRules']().zones.find((zone) => zone.zoneId === 'south-front')?.slots ?? 0;
    for (let index = 0; index < southFrontSlots; index += 1) {
      component['onDefenseDefeatedChanged'](
        'south-front',
        index,
        { target: { checked: true } } as unknown as Event,
      );
    }

    expect(component['isZoneUnlocked']('south-back')).toBe(true);
    expect(component['isZoneUnlocked']('ships')).toBe(false);

    const northSlots =
      component['plannerRules']().zones.find((zone) => zone.zoneId === 'north')?.slots ?? 0;
    for (let index = 0; index < northSlots; index += 1) {
      component['onDefenseDefeatedChanged'](
        'north',
        index,
        { target: { checked: true } } as unknown as Event,
      );
    }

    expect(component['isZoneUnlocked']('ships')).toBe(true);
  });

  it('should allow adding and removing attack options for a specific defense team', () => {
    const fixture = TestBed.createComponent(PlannerSetup);
    const component = fixture.componentInstance;
    fixture.detectChanges();

    component['onSlotAttackOptionDraftChanged'](
      'north',
      0,
      { target: { value: 'Jedi Master Kenobi' } } as unknown as Event,
    );
    component['addSlotAttackOption']('north', 0);
    component['onSlotAttackOptionDraftChanged'](
      'north',
      0,
      { target: { value: 'Starkiller' } } as unknown as Event,
    );
    component['addSlotAttackOption']('north', 0);

    const zoneCards = component['zoneCards']();
    const northSlot = zoneCards.find((zone) => zone.zoneId === 'north')?.teamSlots[0];
    expect(northSlot?.attackOptions).toEqual(['Jedi Master Kenobi', 'Starkiller']);

    component['removeSlotAttackOption']('north', 0, 0);
    const updatedNorthSlot = component['zoneCards']().find((zone) => zone.zoneId === 'north')
      ?.teamSlots[0];
    expect(updatedNorthSlot?.attackOptions).toEqual(['Starkiller']);
  });

  it('should keep attack options isolated by defense slot', () => {
    const fixture = TestBed.createComponent(PlannerSetup);
    const component = fixture.componentInstance;
    fixture.detectChanges();

    component['selectedLeague'].set(League.Bronzium);
    fixture.detectChanges();

    component['onSlotAttackOptionDraftChanged'](
      'north',
      0,
      { target: { value: 'Leia' } } as unknown as Event,
    );
    component['addSlotAttackOption']('north', 0);
    component['onSlotAttackOptionDraftChanged'](
      'north',
      1,
      { target: { value: 'Jabba' } } as unknown as Event,
    );
    component['addSlotAttackOption']('north', 1);

    const northZone = component['zoneCards']().find((zone) => zone.zoneId === 'north');
    expect(northZone?.teamSlots[0]?.attackOptions).toEqual(['Leia']);
    expect(northZone?.teamSlots[1]?.attackOptions).toEqual(['Jabba']);
  });

  it('should disable slot edits while defeated and re-enable when unchecked', () => {
    const fixture = TestBed.createComponent(PlannerSetup);
    const component = fixture.componentInstance;
    fixture.detectChanges();

    component['onDefenseTeamChanged'](
      'north',
      0,
      { target: { value: 'Captain Rex' } } as unknown as Event,
    );
    component['onSlotAttackOptionDraftChanged'](
      'north',
      0,
      { target: { value: 'Jedi Master Kenobi' } } as unknown as Event,
    );
    component['addSlotAttackOption']('north', 0);
    component['onDefenseDefeatedChanged']('north', 0, { target: { checked: true } } as unknown as Event);

    component['onDefenseTeamChanged'](
      'north',
      0,
      { target: { value: 'Changed Name' } } as unknown as Event,
    );
    component['onSlotAttackOptionDraftChanged'](
      'north',
      0,
      { target: { value: 'Starkiller' } } as unknown as Event,
    );
    component['addSlotAttackOption']('north', 0);

    const lockedSlot = component['zoneCards']().find((zone) => zone.zoneId === 'north')?.teamSlots[0];
    expect(lockedSlot?.teamName).toBe('Captain Rex');
    expect(lockedSlot?.attackOptions).toEqual(['Jedi Master Kenobi']);

    component['onDefenseDefeatedChanged']('north', 0, { target: { checked: false } } as unknown as Event);
    component['onDefenseTeamChanged'](
      'north',
      0,
      { target: { value: 'Changed Name' } } as unknown as Event,
    );
    component['onSlotAttackOptionDraftChanged'](
      'north',
      0,
      { target: { value: 'Starkiller' } } as unknown as Event,
    );
    component['addSlotAttackOption']('north', 0);

    const unlockedSlot = component['zoneCards']().find((zone) => zone.zoneId === 'north')?.teamSlots[0];
    expect(unlockedSlot?.teamName).toBe('Changed Name');
    expect(unlockedSlot?.attackOptions).toEqual(['Jedi Master Kenobi', 'Starkiller']);
  });

  it('should persist and restore defense teams and attack options from local storage', () => {
    const fixture = TestBed.createComponent(PlannerSetup);
    const component = fixture.componentInstance;
    fixture.detectChanges();

    component['onDefenseTeamChanged']('north', 0, {
      target: { value: 'General Grievous' },
    } as unknown as Event);
    component['onSlotAttackOptionDraftChanged'](
      'north',
      0,
      { target: { value: 'Jedi Master Kenobi' } } as unknown as Event,
    );
    component['addSlotAttackOption']('north', 0);
    fixture.detectChanges();

    const stored = storage.getItem(DEFENSE_PLAN_STORAGE_KEY);
    expect(stored).not.toBeNull();
    expect(stored).toContain('General Grievous');
    expect(stored).toContain('Jedi Master Kenobi');

    const restoredFixture = TestBed.createComponent(PlannerSetup);
    const restoredComponent = restoredFixture.componentInstance;
    restoredFixture.detectChanges();
    const restoredSlot = restoredComponent['zoneCards']().find((zone) => zone.zoneId === 'north')
      ?.teamSlots[0];

    expect(restoredSlot?.teamName).toBe('General Grievous');
    expect(restoredSlot?.attackOptions).toEqual(['Jedi Master Kenobi']);
  });

  it('should clear only defenses and attack options without clearing mode or league', () => {
    const fixture = TestBed.createComponent(PlannerSetup);
    const component = fixture.componentInstance;
    fixture.detectChanges();

    component['selectedMode'].set(GacMode.ThreeVThree);
    component['selectedLeague'].set(League.Kyber);
    component['onDefenseTeamChanged']('north', 0, { target: { value: 'Reva' } } as unknown as Event);
    component['onSlotAttackOptionDraftChanged'](
      'north',
      0,
      { target: { value: 'Leia' } } as unknown as Event,
    );
    component['addSlotAttackOption']('north', 0);
    component['clearDefenseAndAttackOptions']();
    fixture.detectChanges();

    const slot = component['zoneCards']().find((zone) => zone.zoneId === 'north')?.teamSlots[0];
    expect(slot?.teamName).toBe('');
    expect(slot?.attackOptions).toEqual([]);

    expect(component['selectedMode']()).toBe(GacMode.ThreeVThree);
    expect(component['selectedLeague']()).toBe(League.Kyber);
    expect(storage.getItem(GAC_MODE_STORAGE_KEY)).toBe(GacMode.ThreeVThree);
    expect(storage.getItem(LEAGUE_STORAGE_KEY)).toBe(League.Kyber);

    const storedPlan = storage.getItem(DEFENSE_PLAN_STORAGE_KEY);
    expect(storedPlan).not.toContain('Reva');
    expect(storedPlan).not.toContain('Leia');
  });

  it('should persist and restore attack option draft text', () => {
    const fixture = TestBed.createComponent(PlannerSetup);
    const component = fixture.componentInstance;
    fixture.detectChanges();

    component['onSlotAttackOptionDraftChanged'](
      'north',
      0,
      { target: { value: 'Typed but not added' } } as unknown as Event,
    );
    fixture.detectChanges();

    const restoredFixture = TestBed.createComponent(PlannerSetup);
    const restoredComponent = restoredFixture.componentInstance;
    restoredFixture.detectChanges();

    const restoredSlot = restoredComponent['zoneCards']().find((zone) => zone.zoneId === 'north')
      ?.teamSlots[0];
    expect(restoredSlot?.attackOptionDraft).toBe('Typed but not added');
  });
});
