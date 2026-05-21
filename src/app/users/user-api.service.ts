import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { UserDto } from '../apiModels/user-dto';
import { CreateUserRequest } from '../apiModels/create-user-request';

@Injectable({
  providedIn: 'root',
})
export class UserApiService {
  private readonly httpClient = inject(HttpClient);
  private readonly usersUrl = `${environment.apiBaseUrl}/users`;

  public getUsers(): Observable<UserDto[]> {
    return this.httpClient.get<UserDto[]>(this.usersUrl);
  }

  public createUser(request: CreateUserRequest): Observable<UserDto> {
    return this.httpClient.post<UserDto>(this.usersUrl, request);
  }
}
