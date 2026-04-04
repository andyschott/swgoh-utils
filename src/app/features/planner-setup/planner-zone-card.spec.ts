import { TestBed } from '@angular/core/testing';
import { PlannerZoneCard } from './planner-zone-card';
import { ZoneCardViewModel } from './planner-types';

const zone: ZoneCardViewModel = {
  zoneId: 'north',
  label: 'North',
  slots: 2,
  type: 'Ground',
  isUnlocked: true,
  isCollapsed: false,
  unlockMessage: null,
  teamSlots: [
    {
      teamName: 'Reva',
      defeated: false,
      attackOptions: ['Leia'],
      attackOptionDraft: '',
      isEditable: true,
    },
    {
      teamName: '',
      defeated: false,
      attackOptions: [],
      attackOptionDraft: '',
      isEditable: true,
    },
  ],
};

describe('PlannerZoneCard', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlannerZoneCard],
    }).compileComponents();
  });

  it('should emit collapse toggle request for the zone', () => {
    const fixture = TestBed.createComponent(PlannerZoneCard);
    fixture.componentRef.setInput('zone', zone);
    fixture.detectChanges();

    const emitted: string[] = [];
    fixture.componentInstance.zoneCollapsedToggleRequested.subscribe((value) => emitted.push(value));

    const host = fixture.nativeElement as HTMLElement;
    const toggle = host.querySelector('.zone-toggle') as HTMLButtonElement | null;
    toggle?.click();

    expect(emitted).toEqual(['north']);
  });

  it('should render team slot components for each zone slot', () => {
    const fixture = TestBed.createComponent(PlannerZoneCard);
    fixture.componentRef.setInput('zone', zone);
    fixture.detectChanges();

    const host = fixture.nativeElement as HTMLElement;
    expect(host.querySelectorAll('app-planner-team-slot').length).toBe(2);
  });

  it('should provide an accessible zone-specific label for collapse toggle', () => {
    const fixture = TestBed.createComponent(PlannerZoneCard);
    fixture.componentRef.setInput('zone', zone);
    fixture.detectChanges();

    const host = fixture.nativeElement as HTMLElement;
    const toggle = host.querySelector('.zone-toggle') as HTMLButtonElement | null;
    expect(toggle?.getAttribute('aria-label')).toBe('Collapse North zone');
  });
});
