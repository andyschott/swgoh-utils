import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { Home } from './home';
import { UserApiService } from '../../users/user-api.service';
import { UserDto } from '../../apiModels/user-dto';

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
});
