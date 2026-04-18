import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { MarqueeDatesPage } from './marquee-dates';

describe('MarqueeDatesPage', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MarqueeDatesPage],
      providers: [provideRouter([])],
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

    expect(page.marqueeDates()[0]?.name).toBe('KX Droid');
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
