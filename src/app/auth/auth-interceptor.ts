import { HttpContextToken, HttpEvent, HttpHandlerFn, HttpRequest } from "@angular/common/http";
import { inject } from "@angular/core";
import { AuthService, Token } from "./auth-service";
import { Observable, switchMap } from "rxjs";

export const AUTHENTICATED_REQUEST = new HttpContextToken<boolean>(() => true);

export function authInterceptor(req: HttpRequest<unknown>, next: HttpHandlerFn): Observable<HttpEvent<unknown>> {
    const isAuthenticated = req.context.has(AUTHENTICATED_REQUEST) &&
        req.context.get(AUTHENTICATED_REQUEST);
    if (!isAuthenticated) {
        return next(req);
    }

    const authService = inject(AuthService);
    const token = authService.getToken();
    if (token === null) {
        return next(req);
    }

    if (!authService.isTokenExpired(token)) {
        return next(attachToken(req, token));
    }

    if (authService.isRefreshTokenExpired(token)) {
        authService.logout();
        // TODO: This request will fail - need to handle that gracefully somehow
        return next(req);
    }

    return authService.refresh(token).pipe(
        switchMap((response) => {
            return next(attachToken(req, response));
        })
    );
}

function attachToken(req: HttpRequest<unknown>, token: Token) {
    return req.clone({
        headers: req.headers.set('Authorization', `Bearer ${token.accessToken}`)
    });
}