import { TestBed } from '@angular/core/testing';
import { PlannerAttackOptions } from './planner-attack-options';

describe('PlannerAttackOptions', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlannerAttackOptions],
    }).compileComponents();
  });

  it('should emit draft, add, and remove events', () => {
    const fixture = TestBed.createComponent(PlannerAttackOptions);
    fixture.componentRef.setInput('zoneId', 'north');
    fixture.componentRef.setInput('slotIndex', 0);
    fixture.componentRef.setInput('draft', 'Leia');
    fixture.componentRef.setInput('options', ['JMK', 'Bane']);
    fixture.componentRef.setInput('editable', true);
    fixture.detectChanges();

    const drafts: string[] = [];
    const adds: boolean[] = [];
    const removes: number[] = [];
    fixture.componentInstance.draftChanged.subscribe((value) => drafts.push(value));
    fixture.componentInstance.addRequested.subscribe(() => adds.push(true));
    fixture.componentInstance.removeRequested.subscribe((index) => removes.push(index));

    const host = fixture.nativeElement as HTMLElement;
    const input = host.querySelector('input') as HTMLInputElement;
    const buttons = host.querySelectorAll('button');

    input.value = 'Reva';
    input.dispatchEvent(new Event('input'));
    (buttons[0] as HTMLButtonElement).click();
    (buttons[2] as HTMLButtonElement).click();

    expect(drafts).toEqual(['Reva']);
    expect(adds.length).toBe(1);
    expect(removes).toEqual([1]);
  });

  it('should disable controls when slot is not editable', () => {
    const fixture = TestBed.createComponent(PlannerAttackOptions);
    fixture.componentRef.setInput('zoneId', 'north');
    fixture.componentRef.setInput('slotIndex', 1);
    fixture.componentRef.setInput('draft', '');
    fixture.componentRef.setInput('options', ['SK']);
    fixture.componentRef.setInput('editable', false);
    fixture.detectChanges();

    const host = fixture.nativeElement as HTMLElement;
    const input = host.querySelector('input') as HTMLInputElement | null;
    const buttons = host.querySelectorAll('button');

    expect(input?.disabled).toBe(true);
    for (const button of buttons) {
      expect((button as HTMLButtonElement).disabled).toBe(true);
    }
  });

  it('should render descriptive aria labels for add and remove actions', () => {
    const fixture = TestBed.createComponent(PlannerAttackOptions);
    fixture.componentRef.setInput('zoneId', 'north');
    fixture.componentRef.setInput('slotIndex', 1);
    fixture.componentRef.setInput('draft', '');
    fixture.componentRef.setInput('options', ['JMK']);
    fixture.componentRef.setInput('editable', true);
    fixture.detectChanges();

    const host = fixture.nativeElement as HTMLElement;
    const buttons = host.querySelectorAll('button');
    expect((buttons[0] as HTMLButtonElement).getAttribute('aria-label')).toBe(
      'Add attack option for north team 2',
    );
    expect((buttons[1] as HTMLButtonElement).getAttribute('aria-label')).toBe(
      'Remove attack option JMK',
    );
  });
});
