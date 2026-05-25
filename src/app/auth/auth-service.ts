import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { TokenResponse } from '../apiModels/token-response';
import { LoginRequest } from '../apiModels/login-request';

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

  isLoggedIn() {
    return localStorage.getItem(AuthKey) !== null;
  }

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
    });
  }

  private saveToken(auth: Token) {
    localStorage.setItem(AuthKey, JSON.stringify(auth));
  }
}
