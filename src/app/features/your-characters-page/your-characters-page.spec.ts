import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { Character } from '../../apiModels/character';
import { EarnableLocation } from '../../apiModels/earnable-location';
import { FarmingStatus } from '../../apiModels/farming-status';
import { CharactersApiService } from '../../characters/characters-api.service';
import { YourCharactersPage } from './your-characters-page';

describe('YourCharactersPage', () => {
  const characters: Character[] = [
    {
      id: '2',
      name: 'Zorii Bliss',
      isAccelerated: false,
      locations: [EarnableLocation.CrystalShipments, EarnableLocation.LightSide],
      marquee: null,
      shards: {
        id: 'shards-2',
        shards: 145,
        farmingStatus: FarmingStatus.Active,
      },
    },
    {
      id: '1',
      name: 'Ahsoka Tano',
      isAccelerated: true,
      locations: [EarnableLocation.Cantina],
      marquee: null,
      shards: {
        id: 'shards-1',
        shards: 37,
        farmingStatus: FarmingStatus.Backlog,
      },
    },
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [YourCharactersPage],
      providers: [
        provideRouter([]),
        {
          provide: CharactersApiService,
          useValue: {
            getCharactersForUser: () => of(characters),
          },
        },
      ],
    }).compileComponents();
  });

  it('should create', () => {
    const fixture = TestBed.createComponent(YourCharactersPage);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should render user characters sorted by name with shard totals converted', () => {
    const fixture = TestBed.createComponent(YourCharactersPage);
    fixture.detectChanges();

    const firstRowCells = Array.from(
      fixture.nativeElement.querySelectorAll('tbody tr:first-child th, tbody tr:first-child td'),
    ).map((cell) => (cell as HTMLTableCellElement).textContent?.trim());
    const secondRowCells = Array.from(
      fixture.nativeElement.querySelectorAll('tbody tr:nth-child(2) th, tbody tr:nth-child(2) td'),
    ).map((cell) => (cell as HTMLTableCellElement).textContent?.trim());

    expect(firstRowCells).toEqual(['Ahsoka Tano', '2', '12', '293', 'Backlog', 'Cantina']);
    expect(secondRowCells).toEqual(['Zorii Bliss', '5', '0', '185', 'Active', 'Crystal Shipments, Light Side']);
  });
});
