import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  standalone: true,
  template: '',
})
export class RootRedirectComponent implements OnInit {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  ngOnInit(): void {
    const target = this.auth.isAuthenticated() ? '/sales' : '/login';
    void this.router.navigateByUrl(target, { replaceUrl: true });
  }
}
