import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { ApiError } from '../../core/api-error';
import { UserRole, UserStatus, type UserRoleValue, type UserStatusValue } from '../../models/auth.model';
import type {
  AdminUser,
  UpdateAdminUserPayload,
} from '../../models/admin-user.model';
import { AdminUsersApiService } from '../../services/admin-users-api.service';

const PAGE_SIZES = [10, 20, 50] as const;

const ROLE_OPTIONS: { value: UserRoleValue; label: string }[] = [
  { value: UserRole.Customer, label: 'Cliente' },
  { value: UserRole.Manager, label: 'Gerente' },
  { value: UserRole.Admin, label: 'Administrador' },
];

const STATUS_OPTIONS: { value: UserStatusValue; label: string }[] = [
  { value: UserStatus.Active, label: 'Ativo' },
  { value: UserStatus.Inactive, label: 'Inativo' },
  { value: UserStatus.Suspended, label: 'Suspenso' },
];

function roleLabel(v: UserRoleValue): string {
  return ROLE_OPTIONS.find((o) => o.value === v)?.label ?? String(v);
}

function statusLabel(v: UserStatusValue): string {
  return STATUS_OPTIONS.find((o) => o.value === v)?.label ?? String(v);
}

type EditForm = UpdateAdminUserPayload & { newPassword: string };

function emptyForm(): EditForm {
  return {
    username: '',
    email: '',
    phone: '',
    role: UserRole.Customer,
    status: UserStatus.Active,
    newPassword: '',
  };
}

@Component({
  selector: 'app-admin-users',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './admin-users.component.html',
  styleUrl: './admin-users.component.scss',
})
export class AdminUsersComponent implements OnInit {
  private readonly api = inject(AdminUsersApiService);

  page = 1;
  pageSize = 10;
  readonly rows = signal<AdminUser[]>([]);
  readonly totalPages = signal(1);
  readonly totalItems = signal(0);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  readonly editOpen = signal(false);
  readonly editId = signal<string | null>(null);
  form: EditForm = emptyForm();
  readonly saving = signal(false);

  readonly deleteOpen = signal(false);
  readonly deleteTarget = signal<AdminUser | null>(null);
  readonly deleting = signal(false);

  readonly pageSizes = PAGE_SIZES;
  readonly roleOptions = ROLE_OPTIONS;
  readonly statusOptions = STATUS_OPTIONS;

  rl(v: UserRoleValue): string {
    return roleLabel(v);
  }

  sl(v: UserStatusValue): string {
    return statusLabel(v);
  }

  ngOnInit(): void {
    void this.load();
  }

  async load(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);
    try {
      const res = await firstValueFrom(
        this.api.listAdminUsers(this.page, this.pageSize)
      );
      this.rows.set(res.items);
      this.totalPages.set(Math.max(1, res.totalPages));
      this.totalItems.set(res.totalItems);
    } catch (e) {
      this.error.set(
        e instanceof ApiError ? e.message : 'Falha ao carregar usuários'
      );
      this.rows.set([]);
    } finally {
      this.loading.set(false);
    }
  }

  setPageSize(n: number): void {
    this.pageSize = n;
    this.page = 1;
    void this.load();
  }

  prevPage(): void {
    this.page = Math.max(1, this.page - 1);
    void this.load();
  }

  nextPage(): void {
    this.page = this.page + 1;
    void this.load();
  }

  async openEdit(u: AdminUser): Promise<void> {
    this.error.set(null);
    this.editId.set(u.id);
    this.editOpen.set(true);
    try {
      const fresh = await firstValueFrom(this.api.getAdminUser(u.id));
      this.form = {
        username: fresh.username,
        email: fresh.email,
        phone: fresh.phone,
        role: fresh.role,
        status: fresh.status,
        newPassword: '',
      };
    } catch (e) {
      this.error.set(
        e instanceof ApiError ? e.message : 'Não foi possível carregar o usuário'
      );
      this.editOpen.set(false);
    }
  }

  async submitEdit(): Promise<void> {
    const id = this.editId();
    if (!id) return;
    this.saving.set(true);
    this.error.set(null);
    try {
      const payload: UpdateAdminUserPayload = {
        username: this.form.username.trim(),
        email: this.form.email.trim(),
        phone: this.form.phone.trim().replace(/\s/g, ''),
        role: this.form.role,
        status: this.form.status,
      };
      if (this.form.newPassword.trim())
        payload.newPassword = this.form.newPassword.trim();

      await firstValueFrom(this.api.updateAdminUser(id, payload));
      this.editOpen.set(false);
      this.editId.set(null);
      await this.load();
    } catch (err) {
      this.error.set(
        err instanceof ApiError
          ? err.message
          : 'Não foi possível salvar as alterações'
      );
    } finally {
      this.saving.set(false);
    }
  }

  async confirmDelete(): Promise<void> {
    const t = this.deleteTarget();
    if (!t) return;
    this.deleting.set(true);
    this.error.set(null);
    try {
      await firstValueFrom(this.api.deleteAdminUser(t.id));
      this.deleteOpen.set(false);
      this.deleteTarget.set(null);
      await this.load();
    } catch (e) {
      this.error.set(
        e instanceof ApiError ? e.message : 'Não foi possível excluir'
      );
    } finally {
      this.deleting.set(false);
    }
  }
}
