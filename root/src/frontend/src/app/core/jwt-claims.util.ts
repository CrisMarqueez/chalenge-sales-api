import { parseRoleToValue } from './user-role.util';

const CLAIM_EMAIL =
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress';
const CLAIM_NAME = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';
const CLAIM_ROLE =
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

const EMAIL_KEYS = [CLAIM_EMAIL, 'email', 'Email'] as const;
const NAME_KEYS = [CLAIM_NAME, 'unique_name', 'name', 'Name'] as const;
const ROLE_KEYS = [CLAIM_ROLE, 'role', 'Role'] as const;

export type AuthSessionUser = {
  email: string;
  name: string;
  role: string;
};

function decodeJwtPayload(token: string): Record<string, unknown> | null {
  const parts = token.split('.');
  if (parts.length < 2) return null;
  const b64 = parts[1].replace(/-/g, '+').replace(/_/g, '/');
  const pad = b64.length % 4 === 0 ? '' : '='.repeat(4 - (b64.length % 4));
  try {
    const json = atob(b64 + pad);
    return JSON.parse(json) as Record<string, unknown>;
  } catch {
    return null;
  }
}

function firstClaimString(
  payload: Record<string, unknown>,
  keys: readonly string[]
): string {
  for (const k of keys) {
    const v = payload[k];
    if (v == null || v === '') continue;
    if (Array.isArray(v)) {
      const s = String(v[0] ?? '').trim();
      if (s) return s;
    } else {
      const s = String(v).trim();
      if (s) return s;
    }
  }
  return '';
}

function roleToNumericString(role: string): string {
  const t = role.trim();
  if (!t) return '';
  const v = parseRoleToValue(t);
  return v != null ? String(v) : t;
}

function claimsFromPayload(payload: Record<string, unknown>): AuthSessionUser {
  const rawRole = firstClaimString(payload, ROLE_KEYS);
  return {
    email: firstClaimString(payload, EMAIL_KEYS),
    name: firstClaimString(payload, NAME_KEYS),
    role: roleToNumericString(rawRole),
  };
}

export function parseAuthClaimsFromJwt(token: string): AuthSessionUser | null {
  const payload = decodeJwtPayload(token);
  if (!payload) return null;
  return claimsFromPayload(payload);
}

export function supplementAuthSessionUserFromJwt(
  token: string,
  user: AuthSessionUser
): AuthSessionUser {
  const c = parseAuthClaimsFromJwt(token);
  if (!c) return user;
  return {
    email: user.email?.trim() || c.email,
    name: user.name?.trim() || c.name,
    role: roleToNumericString(user.role?.trim() || c.role),
  };
}
