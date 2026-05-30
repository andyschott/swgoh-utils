import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { map, Observable, of } from 'rxjs';
import { environment } from '../../environments/environment';
import { Ship } from '../apiModels/ship';
import { AUTHENTICATED_REQUEST } from '../auth/auth-interceptor';
import { EarnableShardsRequest } from '../apiModels/earnable-shards-request';
import { EarnableShards } from '../apiModels/earnable-shards';

@Injectable({
  providedIn: 'root',
})
export class ShipsApiService {
  private readonly httpClient = inject(HttpClient);
  private readonly shipsUrl = `${environment.apiBaseUrl}/ships`;
  private readonly shipsForUserUrl = `${environment.apiBaseUrl}/shipsForUser`;

  public getShips(): Observable<Ship[]> {
    return this.httpClient.get<Ship[]>(this.shipsUrl);
  }

  public getShipsForUser(): Observable<Ship[]> {
    return this.httpClient.get<Ship[]>(this.shipsForUserUrl, {
      context: new HttpContext().set(AUTHENTICATED_REQUEST, true)
    });
  }

  public updateShipForUser(ship: Ship): Observable<Ship> {
    if (!ship.shards) {
      return of(ship);
    }

    const request: EarnableShardsRequest = {
      shards: ship.shards.shards,
      farmingStatus: ship.shards.farmingStatus
    }
    return this.httpClient.put<EarnableShards>(`${this.shipsForUserUrl}/${ship.id}`, request)
      .pipe(map((response) => {
        const updatedShip: Ship = {
          id: ship.id,
          name: ship.name,
          locations: ship.locations,
          marquee: ship.marquee,
          shards: {
            id: response.id,
            shards: response.shards,
            farmingStatus: response.farmingStatus
          }
        };

        return updatedShip;
      }));
  }
}
