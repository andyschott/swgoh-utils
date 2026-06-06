import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { EarnableLocation } from '../../apiModels/earnable-location';
import { FarmingStatus } from '../../apiModels/farming-status';
import { Ship } from '../../apiModels/ship';
import { ShipsApiService } from '../../ships/ships-api.service';
import { YourShipsPage } from './your-ships-page';

describe('YourShipsPage', () => {
  const ships: Ship[] = [
    {
      id: '2',
      name: 'TIE Advanced x1',
      locations: [EarnableLocation.Fleet, EarnableLocation.FleetArenaShipments],
      marquee: null,
      shards: {
        id: 'shards-2',
        shards: 145,
        farmingStatus: FarmingStatus.Active,
      },
    },
    {
      id: '1',
      name: "Ahsoka Tano's Jedi Starfighter",
      locations: [EarnableLocation.GalacticWarShipments],
      marquee: null,
      shards: {
        id: 'shards-1',
        shards: 37,
        farmingStatus: FarmingStatus.Backlog,
      },
    },
    {
      id: '3',
      name: "Boba Fett's Slave I",
      locations: [EarnableLocation.FleetArenaShipments],
      marquee: null,
      shards: {
        id: 'shards-3',
        shards: 330,
        farmingStatus: FarmingStatus.Done,
      },
    },
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [YourShipsPage],
      providers: [
        provideRouter([]),
        {
          provide: ShipsApiService,
          useValue: {
            getShipsForUser: () => of(ships),
          },
        },
      ],
    }).compileComponents();
  });

  it('should create', () => {
    const fixture = TestBed.createComponent(YourShipsPage);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should render user ships sorted by farming status and name with shard totals converted', () => {
    const fixture = TestBed.createComponent(YourShipsPage);
    fixture.detectChanges();

    const firstRowCells = Array.from(
      fixture.nativeElement.querySelectorAll('tbody tr:first-child th, tbody tr:first-child td'),
    ).map((cell) => (cell as HTMLTableCellElement).textContent?.trim());
    const secondRowCells = Array.from(
      fixture.nativeElement.querySelectorAll('tbody tr:nth-child(2) th, tbody tr:nth-child(2) td'),
    ).map((cell) => (cell as HTMLTableCellElement).textContent?.trim());
    const thirdRowCells = Array.from(
      fixture.nativeElement.querySelectorAll('tbody tr:nth-child(3) th, tbody tr:nth-child(3) td'),
    ).map((cell) => (cell as HTMLTableCellElement).textContent?.trim());

    expect(firstRowCells).toEqual(['TIE Advanced x1', '5', '0', '185', 'Active', 'Fleet, Fleet Arena Shipments']);
    expect(secondRowCells).toEqual([
      "Ahsoka Tano's Jedi Starfighter",
      '2',
      '12',
      '293',
      'Backlog',
      'Galactic War Shipments',
    ]);
    expect(thirdRowCells).toEqual(["Boba Fett's Slave I", '7', '0', '0', 'Done', 'Fleet Arena Shipments']);
  });
});
