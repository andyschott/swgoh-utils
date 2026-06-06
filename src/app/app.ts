import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs';
import { AppNavData, routes } from './app.routes';
import { AuthService } from './auth/auth-service';

interface NavItem {
  path: string;
  label: string;
}

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrl: './app.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class App {
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);

  protected readonly mobileMenuOpen = signal(false);

  protected readonly navItems = computed<ReadonlyArray<NavItem>>(() =>
    routes
      .filter((route): route is typeof route & { path: string; data: AppNavData } =>
        typeof route.path === 'string' && route.path !== '**' && !!route.data?.['navLabel'],
      )
      .filter((route) => !route.data.requiresAuth || this.isLoggedIn())
      .map((route) => ({
        path: route.path,
        label: route.data.navLabel,
      }))
      .sort((left, right) => {
        if (left.path === '') {
          return -1;
        }

        if (right.path === '') {
          return 1;
        }

        return left.label.localeCompare(right.label, 'en-US');
      }),
  );

  public constructor() {
    this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .pipe(takeUntilDestroyed())
      .subscribe(() => {
        this.closeMobileMenu();
      });
  }

  protected toggleMobileMenu(): void {
    this.mobileMenuOpen.update((value) => !value);
  }

  protected closeMobileMenu(): void {
    this.mobileMenuOpen.set(false);
  }

  protected onNavKeydown(event: KeyboardEvent): void {
    if (event.key === 'Escape') {
      this.closeMobileMenu();
    }
  }

  protected readonly isLoggedIn = computed(() => {
    return this.authService.isLoggedIn();
  })

  protected onLogout() {
    this.authService.logout();
  }
}
