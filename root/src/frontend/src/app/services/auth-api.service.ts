import { inject, Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { ApiClientService } from '../core/api-client.service';
import { parseAuthClaimsFromJwt } from '../core/jwt-claims.util';
import type { DocumentedUserRegistration } from '../models/auth.model';

export type AuthenticateUserRequest = { email: string; password: string };

export type AuthenticateUserResponse = {
  token: string;
  email: string;
  name: string;
  role: string;
};

function authenticateUserResponseFromJwt(
  token: string
): AuthenticateUserResponse {
  const c = parseAuthClaimsFromJwt(token);
  if (!c) throw new Error('Token inválido');
  return {
    token,
    email: c.email,
    name: c.name,
    role: c.role,
  };
}

@Injectable({ providedIn: 'root' })
export class AuthApiService {
  private readonly api = inject(ApiClientService);

  login(body: AuthenticateUserRequest): Observable<AuthenticateUserResponse> {
    return this.api
      .requestJson<{ token: string }>('/auth/login', {
        method: 'POST',
        body: {
          username: body.email,
          password: body.password,
        },
      })
      .pipe(
        map((r) => {
          if (typeof r.token !== 'string' || !r.token)
            throw new Error('Resposta sem token');
          return authenticateUserResponseFromJwt(r.token);
        })
      );
  }

  registerUser(body: DocumentedUserRegistration): Observable<unknown> {
    return this.api.requestJson<unknown>('/users', {
      method: 'POST',
      body,
    });
  }
}
