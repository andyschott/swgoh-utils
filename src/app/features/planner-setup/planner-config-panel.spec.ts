import { TestBed } from '@angular/core/testing';
import { GacMode } from '../../gac/gac-mode.enum';
import { League } from '../../gac/league.enum';
import { PlannerConfigPanel } from './planner-config-panel';

describe('PlannerConfigPanel', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlannerConfigPanel],
    }).compileComponents();
  });

  it('should render selected mode and league', () => {
    const fixture = TestBed.createComponent(PlannerConfigPanel);
    fixture.componentRef.setInput('gacModes', [GacMode.ThreeVThree, GacMode.FiveVFive] as const);
    fixture.componentRef.setInput('leagues', [League.Carbonite, League.Kyber] as const);
    fixture.componentRef.setInput('selectedMode', GacMode.ThreeVThree);
    fixture.componentRef.setInput('selectedLeague', League.Kyber);
    fixture.detectChanges();

    const host = fixture.nativeElement as HTMLElement;
    const modeSelect = host.querySelector('#gac-mode') as HTMLSelectElement | null;
    const leagueSelect = host.querySelector('#league') as HTMLSelectElement | null;

    expect(modeSelect?.value).toBe(GacMode.ThreeVThree);
    expect(leagueSelect?.value).toBe(League.Kyber);
  });

  it('should emit mode and league changes', () => {
    const fixture = TestBed.createComponent(PlannerConfigPanel);
    fixture.componentRef.setInput('gacModes', [GacMode.ThreeVThree, GacMode.FiveVFive] as const);
    fixture.componentRef.setInput('leagues', [League.Carbonite, League.Kyber] as const);
    fixture.componentRef.setInput('selectedMode', GacMode.FiveVFive);
    fixture.componentRef.setInput('selectedLeague', League.Carbonite);
    fixture.detectChanges();

    const emittedModes: GacMode[] = [];
    const emittedLeagues: League[] = [];
    fixture.componentInstance.modeChanged.subscribe((value) => emittedModes.push(value));
    fixture.componentInstance.leagueChanged.subscribe((value) => emittedLeagues.push(value));

    const host = fixture.nativeElement as HTMLElement;
    const modeSelect = host.querySelector('#gac-mode') as HTMLSelectElement;
    const leagueSelect = host.querySelector('#league') as HTMLSelectElement;

    modeSelect.value = GacMode.ThreeVThree;
    modeSelect.dispatchEvent(new Event('change'));

    leagueSelect.value = League.Kyber;
    leagueSelect.dispatchEvent(new Event('change'));

    expect(emittedModes).toEqual([GacMode.ThreeVThree]);
    expect(emittedLeagues).toEqual([League.Kyber]);
  });
});
