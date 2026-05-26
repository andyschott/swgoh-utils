import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Character } from '../apiModels/character';
import { AUTHENTICATED_REQUEST } from '../auth/auth-interceptor';

@Injectable({
  providedIn: 'root',
})
export class CharactersApiService {
  private readonly httpClient = inject(HttpClient);
  private readonly charactersUrl = `${environment.apiBaseUrl}/characters`;

  public getCharacters(): Observable<Character[]> {
    return this.httpClient.get<Character[]>(this.charactersUrl);
  }

  public getCharactersForUser(): Observable<Character[]> {
    return this.httpClient.get<Character[]>(`${environment.apiBaseUrl}/charactersForUser`, {
      context: new HttpContext().set(AUTHENTICATED_REQUEST, true)
    });
  }
}
