import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { MarqueeDate as ApiMarqueeDate } from '../apiModels/marquee-date';
import { DateTime } from 'luxon';

export interface MarqueeDate {
  name: string;
  introduction: Date;
  marqueeEvent: Date;
  shipment: Date;
  farm: Date;
  acceleration: Date | null;
}

@Injectable({
  providedIn: 'root',
})
export class MarqueeDates {
  private readonly httpClient = inject(HttpClient);
  private readonly marqueesUrl = `${environment.apiBaseUrl}/marquees`;

  public getMarqueeDates(): Observable<MarqueeDate[]> {
    return this.httpClient.get<ApiMarqueeDate[]>(this.marqueesUrl)
      .pipe(
        map((marquees) =>
          marquees.map((marquee) => ({
            name: marquee.name,
            introduction: DateTime.fromISO(marquee.introductionDate).toJSDate(),
            marqueeEvent: DateTime.fromISO(marquee.marqueeEventDate).toJSDate(),
            shipment: DateTime.fromISO(marquee.shipmentDate).toJSDate(),
            farm: DateTime.fromISO(marquee.farmDate).toJSDate(),
            acceleration: marquee.accelerationDate === null ? null : DateTime.fromISO(marquee.accelerationDate).toJSDate(),
          }))),
      );
  }
}
