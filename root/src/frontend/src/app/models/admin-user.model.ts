import type { UserRoleValue, UserStatusValue } from './auth.model';

export type AdminUser = {
  id: string;
  username: string;
  name: string;
  email: string;
  phone: string;
  role: UserRoleValue;
  status: UserStatusValue;
};

export type AdminUsersListResult = {
  items: AdminUser[];
  totalItems: number;
  currentPage: number;
  totalPages: number;
};

export type UpdateAdminUserPayload = {
  username: string;
  email: string;
  phone: string;
  role: UserRoleValue;
  status: UserStatusValue;
  newPassword?: string;
};
