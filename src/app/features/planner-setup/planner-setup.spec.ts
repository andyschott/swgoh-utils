import { TestBed } from '@angular/core/testing';
import { GacMode } from '../../gac/gac-mode.enum';
import { League } from '../../gac/league.enum';
import { PlannerSetup } from './planner-setup';

describe('PlannerSetup', () => {
  beforeEach(async () => {
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
});
