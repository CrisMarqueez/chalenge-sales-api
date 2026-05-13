import { computed, inject, Injectable, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { AuthApiService } from './auth-api.service';
import { AuthTokenService, StoredUser } from '../core/auth-token.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly tokens = inject(AuthTokenService);
  private readonly authApi = inject(AuthApiService);
  private readonly revision = signal(0);

  readonly isAuthenticated = computed(() => {
    this.revision();
    return Boolean(this.tokens.getToken());
  });

  readonly user = computed((): StoredUser | null => {
    this.revision();
    return this.tokens.getStoredUser();
  });

  bump(): void {
    this.revision.update((n) => n + 1);
  }

  async login(email: string, password: string): Promise<void> {
    const data = await firstValueFrom(
      this.authApi.login({ email, password })
    );
    this.tokens.setAuthSession(data.token, {
      email: data.email,
      name: data.name,
      role: data.role,
    });
    this.bump();
  }

  logout(): void {
    this.tokens.clearAuthSession();
    this.bump();
  }
}
