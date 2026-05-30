import { signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { App } from './app';
import { routes } from './app.routes';
import { AuthService } from './auth/auth-service';

describe('App', () => {
  const isLoggedIn = signal(false);

  beforeEach(async () => {
    isLoggedIn.set(false);

    await TestBed.configureTestingModule({
      imports: [App],
      providers: [
        provideRouter(routes),
        {
          provide: AuthService,
          useValue: {
            isLoggedIn,
            logout: vi.fn(),
          },
        },
      ],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render the router outlet shell', async () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    await fixture.whenStable();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('router-outlet')).not.toBeNull();
  });

  it('should hide authenticated navigation links when logged out', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const navLinks = Array.from(compiled.querySelectorAll('.site-nav-link')).map((link) => link.textContent?.trim());
    const expectedPublicRoutes = routes.filter((route) => !route.data?.['requiresAuth']);

    expect(navLinks).not.toContain('Your Characters');
    expect(navLinks.length).toBe(expectedPublicRoutes.length);
  });

  it('should render authenticated navigation links when logged in', () => {
    isLoggedIn.set(true);
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const navLinks = Array.from(compiled.querySelectorAll('.site-nav-link')).map((link) => link.textContent?.trim());

    expect(navLinks).toContain('Your Characters');
  });

  it('should toggle mobile navigation menu', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const menuButton = compiled.querySelector('.menu-button') as HTMLButtonElement;
    const nav = compiled.querySelector('.site-nav') as HTMLElement;

    expect(nav.classList.contains('site-nav-open')).toBe(false);
    menuButton.click();
    fixture.detectChanges();
    expect(nav.classList.contains('site-nav-open')).toBe(true);
  });
});
