import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { Home } from './home';
import { UserApiService, type UserDto } from '../../users/user-api.service';

describe('Home', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Home],
      providers: [
        {
          provide: UserApiService,
          useValue: {
            getUsers: (): ReturnType<UserApiService['getUsers']> =>
              of<UserDto[]>([
                { id: 'u-1', email: 'first@example.com' },
                { id: 'u-2', email: 'second@example.com' },
                { id: 'u-3', email: 'third@example.com' },
              ]),
          },
        },
      ],
    }).compileComponents();
  });

  it('should create', () => {
    const fixture = TestBed.createComponent(Home);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should render user count from API results', async () => {
    const fixture = TestBed.createComponent(Home);
    fixture.detectChanges();
    await fixture.whenStable();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Registered Users:');
    expect(compiled.textContent).toContain('3');
  });
});
