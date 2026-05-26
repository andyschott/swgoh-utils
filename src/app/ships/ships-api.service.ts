import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Ship } from '../apiModels/ship';
import { AUTHENTICATED_REQUEST } from '../auth/auth-interceptor';

@Injectable({
  providedIn: 'root',
})
export class ShipsApiService {
  private readonly httpClient = inject(HttpClient);
  private readonly shipsUrl = `${environment.apiBaseUrl}/ships`;

  public getShips(): Observable<Ship[]> {
    return this.httpClient.get<Ship[]>(this.shipsUrl);
  }

  public getShipsForUser(): Observable<Ship[]> {
    return this.httpClient.get<Ship[]>(`${environment.apiBaseUrl}/shipssForUser`, {
      context: new HttpContext().set(AUTHENTICATED_REQUEST, true)
    });
  }
}
