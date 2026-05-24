import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { MarqueeDate as ApiMarqueeDate } from '../apiModels/marquee-date';

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
            introduction: new Date(marquee.introductionDate),
            marqueeEvent: new Date(marquee.marqueeEventDate),
            shipment: new Date(marquee.shipmentDate),
            farm: new Date(marquee.farmDate),
            acceleration: marquee.accelerationDate === null ? null : new Date(marquee.accelerationDate),
          }))),
      );
  }
}
