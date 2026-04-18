import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { RiseOfTheEmpireRewardsPage } from './rise-of-the-empire-rewards';

describe('RiseOfTheEmpireRewardsPage', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RiseOfTheEmpireRewardsPage],
      providers: [provideRouter([])],
    }).compileComponents();
  });

  it('should create', () => {
    const fixture = TestBed.createComponent(RiseOfTheEmpireRewardsPage);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should render rows in descending star order', () => {
    const fixture = TestBed.createComponent(RiseOfTheEmpireRewardsPage);
    fixture.detectChanges();

    const firstRowHeader = fixture.nativeElement.querySelector('tbody tr:first-child th') as HTMLTableCellElement;
    const secondRowHeader = fixture.nativeElement.querySelector('tbody tr:nth-child(2) th') as HTMLTableCellElement;

    expect(firstRowHeader.textContent?.trim()).toBe('56');
    expect(secondRowHeader.textContent?.trim()).toBe('55');
  });
});
