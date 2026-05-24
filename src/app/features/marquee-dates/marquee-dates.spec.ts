import { of } from 'rxjs';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { MarqueeDates } from '../../marquee/marquee-dates';

import { MarqueeDatesPage } from './marquee-dates';

describe('MarqueeDatesPage', () => {
  const marqueeDates = [
    {
      name: 'Imperial Snowtrooper Commander',
      introduction: new Date('2026-04-29'),
      marqueeEvent: new Date('2026-07-14'),
      shipment: new Date('2026-08-05'),
      farm: new Date('2026-10-14'),
      acceleration: new Date('2027-04-29'),
    },
    {
      name: 'Kix',
      introduction: new Date('2018-09-27'),
      marqueeEvent: new Date('2018-09-28'),
      shipment: new Date('2018-11-07'),
      farm: new Date('2018-12-12'),
      acceleration: new Date('2019-09-27'),
    },
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MarqueeDatesPage],
      providers: [
        provideRouter([]),
        {
          provide: MarqueeDates,
          useValue: {
            getMarqueeDates: () => of(marqueeDates),
          },
        },
      ],
    }).compileComponents();
  });

  it('should create', () => {
    const fixture = TestBed.createComponent(MarqueeDatesPage);
    const page = fixture.componentInstance;
    expect(page).toBeTruthy();
  });

  it('should sort by introduction date descending then marquee date descending', () => {
    const fixture = TestBed.createComponent(MarqueeDatesPage);
    const page = fixture.componentInstance as unknown as {
      marqueeDates: () => ReadonlyArray<{ name: string }>;
    };

    expect(page.marqueeDates()[0]?.name).toBe('Imperial Snowtrooper Commander');
  });

  it('should mark future dates', () => {
    const fixture = TestBed.createComponent(MarqueeDatesPage);
    const page = fixture.componentInstance as unknown as {
      isFutureDate: (value: Date | null) => boolean;
    };

    expect(page.isFutureDate(new Date('2999-01-01'))).toBe(true);
    expect(page.isFutureDate(new Date('2000-01-01'))).toBe(false);
    expect(page.isFutureDate(null)).toBe(false);
  });

  it('should filter rows by character name as the user types', () => {
    const fixture = TestBed.createComponent(MarqueeDatesPage);
    const page = fixture.componentInstance as unknown as {
      onSearchInput: (event: Event) => void;
      filteredMarqueeDates: () => ReadonlyArray<{ name: string }>;
    };

    page.onSearchInput({ target: { value: 'kix' } } as unknown as Event);

    expect(page.filteredMarqueeDates().map((item) => item.name)).toEqual(['Kix']);
  });
});
