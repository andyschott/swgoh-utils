import { of } from 'rxjs';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { Character } from '../../apiModels/character';
import { EarnableLocation } from '../../apiModels/earnable-location';
import { CharactersApiService } from '../../characters/characters-api.service';
import { CharactersPage } from './characters-page';

describe('CharactersPage', () => {
  const characters: Character[] = [
    {
      id: '2',
      name: 'Zorii Bliss',
      isAccelerated: false,
      locations: [EarnableLocation.CrystalShipments, EarnableLocation.LightSide],
      marquee: null,
    },
    {
      id: '1',
      name: 'Ahsoka Tano',
      isAccelerated: true,
      locations: [EarnableLocation.Cantina],
      marquee: null,
    },
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CharactersPage],
      providers: [
        provideRouter([]),
        {
          provide: CharactersApiService,
          useValue: {
            getCharacters: () => of(characters),
          },
        },
      ],
    }).compileComponents();
  });

  it('should create', () => {
    const fixture = TestBed.createComponent(CharactersPage);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should render characters sorted by name', () => {
    const fixture = TestBed.createComponent(CharactersPage);
    fixture.detectChanges();

    const firstRowHeader = fixture.nativeElement.querySelector('tbody tr:first-child th') as HTMLTableCellElement;
    const secondRowHeader = fixture.nativeElement.querySelector('tbody tr:nth-child(2) th') as HTMLTableCellElement;

    expect(firstRowHeader.textContent?.trim()).toBe('Ahsoka Tano');
    expect(secondRowHeader.textContent?.trim()).toBe('Zorii Bliss');
  });
});
