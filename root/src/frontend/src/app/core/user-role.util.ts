export const UserRole = {
  None: 0,
  Customer: 1,
  Manager: 2,
  Admin: 3,
} as const;

export type UserRoleValue = (typeof UserRole)[keyof typeof UserRole];

export function parseRoleToValue(
  role: string | number | undefined | null
): UserRoleValue | null {
  if (role == null || role === '') return null;
  if (typeof role === 'number') {
    if (role === UserRole.None) return UserRole.None;
    if (role === UserRole.Customer) return UserRole.Customer;
    if (role === UserRole.Manager) return UserRole.Manager;
    if (role === UserRole.Admin) return UserRole.Admin;
    return null;
  }
  const t = String(role).trim();
  if (/^\d+$/.test(t)) {
    const n = Number(t);
    if (n === UserRole.None) return UserRole.None;
    if (n === UserRole.Customer) return UserRole.Customer;
    if (n === UserRole.Manager) return UserRole.Manager;
    if (n === UserRole.Admin) return UserRole.Admin;
    return null;
  }
  const lower = t.toLowerCase();
  if (lower === 'admin') return UserRole.Admin;
  if (lower === 'manager') return UserRole.Manager;
  if (lower === 'customer') return UserRole.Customer;
  if (lower === 'none') return UserRole.None;
  return null;
}

export function isAdminRole(role: string | number | undefined | null): boolean {
  return parseRoleToValue(role) === UserRole.Admin;
}
