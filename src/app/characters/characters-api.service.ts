import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { map, Observable, of } from 'rxjs';
import { environment } from '../../environments/environment';
import { Character } from '../apiModels/character';
import { AUTHENTICATED_REQUEST } from '../auth/auth-interceptor';
import { EarnableShardsRequest } from '../apiModels/earnable-shards-request';
import { ShardConverter } from '../earnables/shard-converter';
import { EarnableShards } from '../apiModels/earnable-shards';

@Injectable({
  providedIn: 'root',
})
export class CharactersApiService {
  private readonly httpClient = inject(HttpClient);
  private readonly charactersUrl = `${environment.apiBaseUrl}/characters`;
  private readonly charactersForUserUrl = `${environment.apiBaseUrl}/charactersForUser`;

  public getCharacters(): Observable<Character[]> {
    return this.httpClient.get<Character[]>(this.charactersUrl);
  }

  public getCharactersForUser(): Observable<Character[]> {
    return this.httpClient.get<Character[]>(this.charactersForUserUrl, {
      context: new HttpContext().set(AUTHENTICATED_REQUEST, true)
    });
  }

  public updateCharacterForUser(character: Character): Observable<Character> {
    if (!character.shards) {
      return of(character);
    }

    const request: EarnableShardsRequest = {
      shards: character.shards.shards,
      farmingStatus: character.shards.farmingStatus
    }
    return this.httpClient.put<EarnableShards>(`${this.charactersForUserUrl}/${character.id}`, request)
      .pipe(map((response) => {
        const updatedCharacter: Character = {
          id: character.id,
          isAccelerated: character.isAccelerated,
          name: character.name,
          locations: character.locations,
          marquee: character.marquee,
          shards: {
            id: response.id,
            shards: response.shards,
            farmingStatus: response.farmingStatus
          }
        };

        return updatedCharacter;
      }));
  }
}
