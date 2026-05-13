import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { ApiClientService } from '../core/api-client.service';
import {
  tryParseDocumentedPagedList,
  unwrapApiEnvelope,
} from '../core/sale-payload.util';
import type {
  AdminUser,
  AdminUsersListResult,
  UpdateAdminUserPayload,
} from '../models/admin-user.model';
import type { ApiResponseWithData } from '../models/auth.model';
import type { UserRoleValue, UserStatusValue } from '../models/auth.model';

function parseAdminUser(row: unknown): AdminUser {
  if (!row || typeof row !== 'object') {
    return {
      id: '',
      username: '',
      name: '',
      email: '',
      phone: '',
      role: 0 as UserRoleValue,
      status: 0 as UserStatusValue,
    };
  }
  const r = row as Record<string, unknown>;
  return {
    id: String(r['id'] ?? r['Id'] ?? ''),
    username: String(r['username'] ?? r['Username'] ?? ''),
    name: String(r['name'] ?? r['Name'] ?? r['username'] ?? ''),
    email: String(r['email'] ?? r['Email'] ?? ''),
    phone: String(r['phone'] ?? r['Phone'] ?? ''),
    role: Number(r['role'] ?? r['Role'] ?? 0) as UserRoleValue,
    status: Number(r['status'] ?? r['Status'] ?? 0) as UserStatusValue,
  };
}

function parsePagedList(
  raw: unknown,
  requestedPage: number
): AdminUsersListResult {
  const documented = tryParseDocumentedPagedList(raw, requestedPage);
  if (documented) {
    return {
      items: documented.data.map(parseAdminUser),
      totalItems: documented.totalItems,
      currentPage: documented.currentPage,
      totalPages: documented.totalPages,
    };
  }

  if (!raw || typeof raw !== 'object') {
    return {
      items: [],
      totalItems: 0,
      currentPage: 1,
      totalPages: 1,
    };
  }
  const o = raw as Record<string, unknown>;
  const dataField = o['data'] ?? o['Data'];

  let items: AdminUser[] = [];
  if (Array.isArray(dataField)) {
    items = dataField.map(parseAdminUser);
  } else if (dataField && typeof dataField === 'object' && !Array.isArray(dataField)) {
    const nested = dataField as Record<string, unknown>;
    const innerList = nested['data'] ?? nested['Data'];
    if (Array.isArray(innerList)) items = innerList.map(parseAdminUser);
  }

  return {
    items,
    totalItems: Number(o['totalItems'] ?? o['TotalItems'] ?? items.length),
    currentPage: Number(o['currentPage'] ?? o['CurrentPage'] ?? 1),
    totalPages: Math.max(1, Number(o['totalPages'] ?? o['TotalPages'] ?? 1)),
  };
}

function unwrapUserPayload(raw: unknown): AdminUser {
  const inner = unwrapApiEnvelope(raw);
  const payload =
    inner != null && typeof inner === 'object' && !Array.isArray(inner)
      ? inner
      : raw;
  if (!payload || typeof payload !== 'object') return parseAdminUser(null);
  const o = payload as Record<string, unknown>;
  const data = (o['data'] ?? o['Data'] ?? o) as Record<string, unknown>;
  return parseAdminUser(data);
}

@Injectable({ providedIn: 'root' })
export class AdminUsersApiService {
  private readonly api = inject(ApiClientService);

  listAdminUsers(
    page: number,
    pageSize: number
  ): Observable<AdminUsersListResult> {
    const q = new URLSearchParams({
      _page: String(page),
      _size: String(pageSize),
    });
    return this.api
      .requestJson<unknown>(`/api/admin/users?${q}`)
      .pipe(map((raw) => parsePagedList(raw, page)));
  }

  getAdminUser(id: string): Observable<AdminUser> {
    return this.api.requestJson<unknown>(`/api/admin/users/${id}`).pipe(
      map((raw) => {
        if (raw && typeof raw === 'object' && 'data' in (raw as object)) {
          const w = raw as ApiResponseWithData<unknown>;
          if (w.data) return parseAdminUser(w.data);
        }
        return unwrapUserPayload(raw);
      })
    );
  }

  updateAdminUser(
    id: string,
    body: UpdateAdminUserPayload
  ): Observable<AdminUser> {
    const payload: Record<string, unknown> = {
      username: body.username,
      email: body.email,
      phone: body.phone,
      role: body.role,
      status: body.status,
    };
    if (body.newPassword?.trim())
      payload['newPassword'] = body.newPassword.trim();

    return this.api
      .requestJson<unknown>(`/api/admin/users/${id}`, {
        method: 'PUT',
        body: payload,
      })
      .pipe(
        map((raw) => {
          if (raw && typeof raw === 'object' && 'data' in (raw as object)) {
            const w = raw as ApiResponseWithData<unknown>;
            if (w.data) return parseAdminUser(w.data);
          }
          return unwrapUserPayload(raw);
        })
      );
  }

  deleteAdminUser(id: string): Observable<void> {
    return this.api
      .requestJson<unknown>(`/api/admin/users/${id}`, { method: 'DELETE' })
      .pipe(map(() => undefined));
  }
}
