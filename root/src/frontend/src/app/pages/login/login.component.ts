import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiError } from '../../core/api-error';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent implements OnInit {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  email = '';
  password = '';
  readonly error = signal<string | null>(null);
  readonly loading = signal(false);

  readonly registered =
    this.route.snapshot.queryParamMap.get('registered') === '1';

  ngOnInit(): void {
    if (this.auth.isAuthenticated()) {
      void this.router.navigateByUrl(
        this.safeReturnPath(this.route.snapshot.queryParamMap.get('from')),
        { replaceUrl: true }
      );
    }
  }

  private safeReturnPath(raw: string | null): string {
    if (!raw || !raw.startsWith('/') || raw.startsWith('//')) return '/sales';
    try {
      const u = new URL(raw, window.location.origin);
      if (u.pathname === '/login' || u.pathname === '/cadastro') return '/sales';
      return `${u.pathname}${u.search}`;
    } catch {
      return '/sales';
    }
  }

  async submit(): Promise<void> {
    this.error.set(null);
    this.loading.set(true);
    try {
      await this.auth.login(this.email.trim(), this.password);
      const next = this.safeReturnPath(
        this.route.snapshot.queryParamMap.get('from')
      );
      await this.router.navigateByUrl(next, { replaceUrl: true });
    } catch (err) {
      this.error.set(
        err instanceof ApiError
          ? err.message
          : 'Não foi possível entrar. Verifique suas credenciais.'
      );
    } finally {
      this.loading.set(false);
    }
  }
}
