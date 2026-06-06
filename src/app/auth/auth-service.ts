import { HttpClient, HttpContext } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { TokenResponse } from '../apiModels/token-response';
import { LoginRequest } from '../apiModels/login-request';
import { RevokeRequest } from '../apiModels/revoke-request';
import { RefreshRequest } from '../apiModels/refresh-request';
import { map, Observable, of } from 'rxjs';
import { AUTHENTICATED_REQUEST } from './auth-interceptor';

const AuthKey = "authentication";
const TokenExpirationPadding = 300;

export interface Token {
  accessToken: string;
  refreshToken: string;
  expiresAt: Date;
  refreshTokenExpiresAt: Date;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly httpClient = inject(HttpClient);
  private readonly authUrl = `${environment.apiBaseUrl}/auth`;

  private readonly _isLoggedIn = signal(this.hasSavedToken());
  readonly isLoggedIn = this._isLoggedIn.asReadonly();

  public static readonly authenticatedContext = new HttpContext()
    .set(AUTHENTICATED_REQUEST, true);

  login(username: string, password: string) {
    const request: LoginRequest = {
      email: username,
      password
    };
    this.httpClient.post<TokenResponse>(`${this.authUrl}/login`, request)
      .subscribe((response) => {
        const expiresAt = new Date();
        expiresAt.setSeconds(expiresAt.getSeconds() + response.expiresIn - TokenExpirationPadding);

        const refreshTokenExpiresAt = new Date();
        refreshTokenExpiresAt.setSeconds(refreshTokenExpiresAt.getSeconds() + response.refreshTokenExpiresIn - TokenExpirationPadding);

        const auth: Token = {
          accessToken: response.accessToken,
          refreshToken: response.refreshToken,
          expiresAt,
          refreshTokenExpiresAt,
        };
        this.saveToken(auth);
      });
  }

  refresh(token: Token): Observable<Token> {
    const request: RefreshRequest = {
      refreshToken: token.refreshToken
    };
    return this.httpClient.post<TokenResponse>(`${this.authUrl}/refresh`, request).pipe(
      map((response) => {
        const expiresAt = new Date();
        expiresAt.setSeconds(expiresAt.getSeconds() + response.expiresIn - TokenExpirationPadding);

        const refreshTokenExpiresAt = new Date();
        refreshTokenExpiresAt.setSeconds(refreshTokenExpiresAt.getSeconds() + response.refreshTokenExpiresIn - TokenExpirationPadding);

        const auth: Token = {
          accessToken: response.accessToken,
          refreshToken: response.refreshToken,
          expiresAt,
          refreshTokenExpiresAt,
        };
        this.saveToken(auth);

        return auth;
      }));
  }

  logout() {
    const token = this.getToken();
    if (token === null) {
      return;
    }

    const request: RevokeRequest = {
      refreshToken: token.refreshToken
    };
    this.httpClient.post(`${this.authUrl}/revoke`, request)
      .subscribe(() => {
        this.clearToken();
      });
  }

  getToken(): Token | null {
    const item = localStorage.getItem(AuthKey);
    if (item === null) {
      return null;
    }

    return JSON.parse(item) as Token;
  }

  isTokenExpired(token: Token) {
    return new Date() > token.expiresAt;
  }

  isRefreshTokenExpired(token: Token) {
    return new Date() > token.refreshTokenExpiresAt;
  }

  private saveToken(auth: Token) {
    localStorage.setItem(AuthKey, JSON.stringify(auth));
    this._isLoggedIn.set(true);
  }

  private clearToken() {
    localStorage.removeItem(AuthKey);
    this._isLoggedIn.set(false);
  }

  private hasSavedToken(): boolean {
    const token = this.getToken();
    if (token === null) {
      return false;
    }

    return !this.isTokenExpired(token) &&
      !this.isRefreshTokenExpired(token);
  }
}
