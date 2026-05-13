import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
} from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { ApiError, formatErrorBody } from './api-error';
import { AuthTokenService } from './auth-token.service';

@Injectable({ providedIn: 'root' })
export class ApiClientService {
  private readonly http = inject(HttpClient);
  private readonly authToken = inject(AuthTokenService);

  apiBase(): string {
    return environment.apiBaseUrl.replace(/\/$/, '');
  }

  requestJson<T>(
    path: string,
    init?: { method?: string; body?: unknown; headers?: Record<string, string> }
  ): Observable<T> {
    const url = `${this.apiBase()}${path.startsWith('/') ? path : `/${path}`}`;
    const method = (init?.method ?? 'GET').toUpperCase();
    let headers = new HttpHeaders({ Accept: 'application/json' });
    if (init?.body != null && method !== 'GET' && method !== 'HEAD') {
      headers = headers.set('Content-Type', 'application/json');
    }
    if (init?.headers) {
      for (const [k, v] of Object.entries(init.headers)) {
        headers = headers.set(k, v);
      }
    }
    const token = this.authToken.getToken();
    if (token && !headers.has('Authorization')) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }

    return this.http
      .request<T>(method, url, {
        body: init?.body ?? undefined,
        headers,
        observe: 'body',
      })
      .pipe(
        catchError((err: HttpErrorResponse) => {
          const body = err.error;
          const apiErr = new ApiError(err.status, body);
          apiErr.message =
            formatErrorBody(body, err.status, err.statusText) || apiErr.message;
          return throwError(() => apiErr);
        })
      );
  }
}
