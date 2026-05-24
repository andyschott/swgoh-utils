import { of } from 'rxjs';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { EarnableLocation } from '../../apiModels/earnable-location';
import { Ship } from '../../apiModels/ship';
import { ShipsApiService } from '../../ships/ships-api.service';
import { ShipsPage } from './ships-page';

describe('ShipsPage', () => {
  const ships: Ship[] = [
    {
      id: '2',
      name: 'Razor Crest',
      locations: ['Fleet' as unknown as EarnableLocation],
      marquee: null,
    },
    {
      id: '1',
      name: 'Ahsoka Tano\'s Jedi Starfighter',
      locations: ['FleetArenaShipments' as unknown as EarnableLocation],
      marquee: null,
    },
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShipsPage],
      providers: [
        provideRouter([]),
        {
          provide: ShipsApiService,
          useValue: {
            getShips: () => of(ships),
          },
        },
      ],
    }).compileComponents();
  });

  it('should create', () => {
    const fixture = TestBed.createComponent(ShipsPage);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should render ships sorted by name', () => {
    const fixture = TestBed.createComponent(ShipsPage);
    fixture.detectChanges();

    const firstRowHeader = fixture.nativeElement.querySelector('tbody tr:first-child th') as HTMLTableCellElement;
    const secondRowHeader = fixture.nativeElement.querySelector('tbody tr:nth-child(2) th') as HTMLTableCellElement;

    expect(firstRowHeader.textContent?.trim()).toBe('Ahsoka Tano\'s Jedi Starfighter');
    expect(secondRowHeader.textContent?.trim()).toBe('Razor Crest');
  });
});
