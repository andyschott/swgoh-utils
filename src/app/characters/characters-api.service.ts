import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Character } from '../apiModels/character';

@Injectable({
  providedIn: 'root',
})
export class CharactersApiService {
  private readonly httpClient = inject(HttpClient);
  private readonly charactersUrl = `${environment.apiBaseUrl}/characters`;

  public getCharacters(): Observable<Character[]> {
    return this.httpClient.get<Character[]>(this.charactersUrl);
  }
}
