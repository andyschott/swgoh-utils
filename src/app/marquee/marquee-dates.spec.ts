import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { environment } from '../../environments/environment';
import { MarqueeDates } from './marquee-dates';

describe('MarqueeDates', () => {
  let service: MarqueeDates;
  let httpTestingController: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(MarqueeDates);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTestingController.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should map api response dates', () => {
    let result:
      | Array<{
          name: string;
          introduction: Date;
          marqueeEvent: Date;
          shipment: Date;
          farm: Date;
          acceleration: Date | null;
        }>
      | undefined;

    service.getMarqueeDates().subscribe((value) => {
      result = value;
    });

    const request = httpTestingController.expectOne(`${environment.apiBaseUrl}/marquees`);
    expect(request.request.method).toBe('GET');

    request.flush([
      {
        name: 'Zorii Bliss',
        introductionDate: '2023-01-11',
        marqueeEventDate: '2023-01-12',
        shipmentDate: '2023-02-08',
        farmDate: '2023-03-22',
        accelerationDate: '2024-01-11',
      },
    ]);

    expect(result?.[0]?.name).toBe('Zorii Bliss');
    expect(result?.[0]?.introduction.toISOString().slice(0, 10)).toBe('2023-01-11');
    expect(result?.[0]?.acceleration?.toISOString().slice(0, 10)).toBe('2024-01-11');
  });

  it('should map null acceleration', () => {
    let result:
      | Array<{
          name: string;
          introduction: Date;
          marqueeEvent: Date;
          shipment: Date;
          farm: Date;
          acceleration: Date | null;
        }>
      | undefined;

    service.getMarqueeDates().subscribe((value) => {
      result = value;
    });

    const request = httpTestingController.expectOne(`${environment.apiBaseUrl}/marquees`);
    request.flush([
      {
        name: 'Outrider',
        introductionDate: '2022-04-07',
        marqueeEventDate: '2022-04-08',
        shipmentDate: '2022-05-11',
        farmDate: '2022-06-08',
        accelerationDate: null,
      },
    ]);

    expect(result?.[0]?.acceleration).toBeNull();
  });
});
