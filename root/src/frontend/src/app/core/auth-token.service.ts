import { Injectable } from '@angular/core';
import { supplementAuthSessionUserFromJwt } from './jwt-claims.util';

const TOKEN_KEY = 'dev_eval_token';
const USER_KEY = 'dev_eval_user';

export type StoredUser = {
  email: string;
  name: string;
  role: string;
};

@Injectable({ providedIn: 'root' })
export class AuthTokenService {
  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  setAuthSession(token: string, user: StoredUser): void {
    localStorage.setItem(TOKEN_KEY, token);
    localStorage.setItem(USER_KEY, JSON.stringify(user));
    window.dispatchEvent(new Event('dev-eval-auth'));
  }

  getStoredUser(): StoredUser | null {
    const raw = localStorage.getItem(USER_KEY);
    if (!raw) return null;
    try {
      const user = JSON.parse(raw) as StoredUser;
      const token = this.getToken();
      if (token) return supplementAuthSessionUserFromJwt(token, user);
      return user;
    } catch {
      return null;
    }
  }

  clearAuthSession(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    window.dispatchEvent(new Event('dev-eval-auth'));
  }
}
