import { HttpContextToken, HttpHandlerFn, HttpRequest } from "@angular/common/http";
import { inject } from "@angular/core";
import { AuthService } from "./auth-service";

export const AUTHENTICATED_REQUEST = new HttpContextToken<boolean>(() => true);

export function authInterceptor(req: HttpRequest<unknown>, next: HttpHandlerFn) {
    if (!req.context.get(AUTHENTICATED_REQUEST)) {
        return next(req);
    }

    const authService = inject(AuthService);
    const token = authService.getToken();
    if (token === null) {
        return next(req);
    }

    // TODO: Implement token refreshing
    const authenticatedRequest = req.clone({
        headers: req.headers.set('Authentication', `Bearer ${token.accessToken}`)
    });

    return next(req);
}