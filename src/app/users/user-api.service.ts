import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface UserDto {
  id: string;
  email: string;
}

export interface CreateUserRequest {
  email: string;
  password: string;
}

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
