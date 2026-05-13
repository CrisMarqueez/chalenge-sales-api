import { Component, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import {
  NavigationEnd,
  Router,
  RouterLink,
  RouterLinkActive,
  RouterOutlet,
} from '@angular/router';
import { filter, map, startWith } from 'rxjs';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  readonly auth = inject(AuthService);
  readonly router = inject(Router);

  readonly title = 'DeveloperStore — Vendas';

  private readonly url = toSignal(
    this.router.events.pipe(
      filter((e): e is NavigationEnd => e instanceof NavigationEnd),
      map(() => this.router.url),
      startWith(this.router.url)
    ),
    { initialValue: this.router.url }
  );

  readonly saleFormActive = computed(() => {
    const path = this.url();
    return path === '/sales/new' || /\/sales\/[^/]+\/edit$/.test(path);
  });

  /** Telas de autenticação com layout próprio (sem header global). */
  readonly authLayout = computed(() => {
    const path = this.url().split('?')[0] ?? '';
    return path === '/login' || path === '/cadastro';
  });
}
