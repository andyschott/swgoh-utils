import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { Login } from '../../login/login';
import { AuthService } from '../../auth/auth-service';

@Component({
  selector: 'app-home',
  imports: [Login],
  templateUrl: './home.html',
  styleUrl: './home.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Home {
  private readonly authService = inject(AuthService);

  protected readonly isLoggedIn = computed(() => {
    return this.authService.isLoggedIn();
  })
}
