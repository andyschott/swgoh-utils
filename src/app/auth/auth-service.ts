import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { TokenResponse } from '../apiModels/token-response';
import { LoginRequest } from '../apiModels/login-request';
import { map, Observable } from 'rxjs';

const AuthKey = "authentication";

interface Token {
  accessToken: string;
  refreshToken: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly httpClient = inject(HttpClient);
  private readonly authUrl = `${environment.apiBaseUrl}/auth`;

  private readonly _isLoggedIn = signal(this.hasSavedToken());
  readonly isLoggedIn = this._isLoggedIn.asReadonly();

  login(username: string, password: string) {
    const request: LoginRequest = {
      email: username,
      password
    };
    this.httpClient.post<TokenResponse>(`${this.authUrl}/login`, request)
      .subscribe((response) => {
        const auth: Token = {
          accessToken: response.accessToken,
          refreshToken: response.refreshToken
        };
        this.saveToken(auth);
        this._isLoggedIn.set(true);
      });
  }

  private saveToken(auth: Token) {
    localStorage.setItem(AuthKey, JSON.stringify(auth));
  }

  private hasSavedToken() {
    return localStorage.getItem(AuthKey) !== null;
  }
}
